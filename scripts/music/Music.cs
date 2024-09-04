using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Project;

public partial class Music : Node
{
	[Signal] public delegate void BeatTickEventHandler(BeatTime beat);
	[Signal] public delegate void BeatWindowUnlockEventHandler(BeatTime beat);
	[Signal] public delegate void BeatWindowLockEventHandler(BeatTime beat);
	[Signal] public delegate void CurrentTrackPositionChangedEventHandler(double beatIndex);

	private MusicLibrary musicLibrary = new();

	public readonly float SongDelay = 2; // seconds
	public const float MinBeatSize = 0.25f;
	public double StartingFromBeat = 0;

	public float BeatsPerMinute
	{
		get => CurrentTrack != null ? CurrentTrack.BeatsPerMinute : 60;
	}
	public float BeatsPerSecond
	{
		get => BeatsPerMinute / 60;
	}
	public float SecondsPerBeat
	{
		get => 1 / BeatsPerSecond;
	}
	public float GameSpeed
	{
		get => BeatsPerMinute / 60;
	}
	bool IsStarted = false;
	bool IsFadingOut = false;

	public bool IsPlaying
	{
		get => IsStarted && PreciseBeatIndex > 0;
	}

	private float LongestCalibration;
	public AccurateTimer BeatTimer;
	public AccurateTimer VisualBeatTimer;

	public long PreciseBeatIndex = -1;
	public float PredictiveBeatTime = 0;
	public double BeatIndex { get => PreciseBeatIndex * MinBeatSize; }
	public float TimingWindow { get => AccurateTimer.TimingWindow; }
	public float TimingWindowMs { get => AccurateTimer.TimingWindow * 1000f; }
	public float QueueingWindow { get => AccurateTimer.QueueingWindow; }
	public MusicTrack CurrentTrack;

	readonly Dictionary<BeatTime, List<(Action<BeatTime> callback, float timeBefore)>> PredictiveBeatCallbacks = new();

	public override void _EnterTree()
	{
		instance = this;
	}

	public override void _Ready()
	{
		PredictiveBeatCallbacks[BeatTime.Whole] = new();
		PredictiveBeatCallbacks[BeatTime.Half] = new();
		PredictiveBeatCallbacks[BeatTime.Quarter] = new();
		PredictiveBeatCallbacks[BeatTime.Eighth] = new();
		PredictiveBeatCallbacks[BeatTime.Sixteenth] = new();

		AddChild(musicLibrary);

		BeatTimer = new AccurateTimer
		{
			Calibration = 0,
			BeatTime = BeatTime.Sixteenth,
		};
		AddChild(BeatTimer);

		VisualBeatTimer = new AccurateTimer
		{
			Calibration = SongDelay,
			BeatTime = BeatTime.Quarter,
		};
		AddChild(VisualBeatTimer);

		// Ensure no timer starts in the future
		List<AccurateTimer> timers = new() { BeatTimer, VisualBeatTimer };
		LongestCalibration = timers.OrderByDescending(timer => timer.Calibration).ToList()[0].Calibration;
		foreach (var timer in timers)
			timer.Calibration -= LongestCalibration;

		BeatTimer.Timeout += OnInternalTimerTimeout;
		BeatTimer.CatchUpTick += (_) => PreciseBeatIndex += 1;

		LoadingManager.Singleton.SceneTransitionStarted += OnSceneTransitionStarted;
		LoadingManager.Singleton.SceneTransitionFinished += OnSceneTransitionFinished;

		var scene = Lib.Scene.ToEnum(GetTree().CurrentScene.SceneFilePath);
		PrepareSceneSong(scene);
		PlaySceneSong(scene);
	}

	private void OnInternalTimerTimeout(BeatTime beat)
	{
		PreciseBeatIndex += 1;
		var beatTime = TickIndexToBeatTime(PreciseBeatIndex);
		try
		{
			EmitSignal(SignalName.BeatTick, beatTime.ToVariant());
		}
		catch (Exception ex) { GD.PrintErr(ex); }

		var upcomingBeats = GetUpcomingBeats();

		var engineTime = CastUtils.GetEngineTime();
		foreach (var upcomingBeat in upcomingBeats)
		{
			if (PredictiveBeatTime >= engineTime + upcomingBeat.timeUntil)
				continue;

			PredictiveBeatTime = engineTime + upcomingBeat.timeUntil;
			foreach (var callback in PredictiveBeatCallbacks[upcomingBeat.beatTime])
			{
				new Action(async () =>
				{
					await ToSignal(GetTree().CreateTimer(upcomingBeat.timeUntil - callback.timeBefore), "timeout".ToStringName());
					// Check if the callback was removed before the timeout
					if (PredictiveBeatCallbacks[upcomingBeat.beatTime].Contains(callback))
						callback.callback(upcomingBeat.beatTime);
				}).Invoke();
			}
		}
	}

	private static BeatTime TickIndexToBeatTime(long tickIndex)
	{
		if (tickIndex % (int)BeatTime.Whole == 0)
			return BeatTime.Whole;
		else if (tickIndex % (int)BeatTime.Half == 0)
			return BeatTime.Half;
		else if (tickIndex % (int)BeatTime.Quarter == 0)
			return BeatTime.Quarter;
		else if (tickIndex % (int)BeatTime.Eighth == 0)
			return BeatTime.Eighth;
		else
			return BeatTime.Sixteenth;
	}

	public List<(BeatTime beatTime, float timeUntil)> GetUpcomingBeats()
	{
		var beats = new List<(BeatTime, float)>();
		var engineTime = CastUtils.GetEngineTime();
		for (var i = 1; i < 3; i++)
		{
			var nextBeatTime = TickIndexToBeatTime(PreciseBeatIndex + i);
			var nextBeatTimingMs = BeatTimer.LastTickedAt + MinBeatSize * i - engineTime;
			beats.Add((nextBeatTime, nextBeatTimingMs));
		}

		return beats;
	}

	public void OnBeforeBeatTick(BeatTime beatTime, Action<BeatTime> callback, float timeBefore)
	{
		if (beatTime.Has(BeatTime.Whole))
			PredictiveBeatCallbacks[BeatTime.Whole].Add((callback, timeBefore));
		if (beatTime.Has(BeatTime.Half))
			PredictiveBeatCallbacks[BeatTime.Half].Add((callback, timeBefore));
		if (beatTime.Has(BeatTime.Quarter))
			PredictiveBeatCallbacks[BeatTime.Quarter].Add((callback, timeBefore));
		if (beatTime.Has(BeatTime.Eighth))
			PredictiveBeatCallbacks[BeatTime.Eighth].Add((callback, timeBefore));
		if (beatTime.Has(BeatTime.Sixteenth))
			PredictiveBeatCallbacks[BeatTime.Sixteenth].Add((callback, timeBefore));
	}

	public void OffBeforeBeatTick(Action<BeatTime> callback)
	{
		foreach (var beatTime in PredictiveBeatCallbacks.Keys)
		{
			PredictiveBeatCallbacks[beatTime].RemoveAll(tuple => tuple.callback == callback);
		}
	}

	private void PrepareSceneSong(PlayableScene scene)
	{
		CurrentTrack = scene == PlayableScene.BossArenaAeriel ? musicLibrary.BossArenaAeriel : musicLibrary.TrainingRoom;
	}

	private async void PlaySceneSong(PlayableScene scene)
	{
		if (scene is PlayableScene.TrainingRoom)
		{
			await ToSignal(GetTree().CreateTimer(1), "timeout".ToStringName());
			Start();
		}
	}

	public void Start()
	{
		if (IsStarted)
			return;

		var startTime = (float)StartingFromBeat * SecondsPerBeat;
		CurrentTrack.PlayAfterDelay(SongDelay, startTime);
		CurrentTrack.Volume = Preferences.Singleton.MusicVolume;

		PreciseBeatIndex = (long)Math.Round(StartingFromBeat / MinBeatSize) - 1;
		PredictiveBeatTime = CastUtils.GetEngineTime();
		IsStarted = true;
		IsFadingOut = false;

		BeatTimer.Start(BeatsPerMinute * 4f);
		VisualBeatTimer.Start(BeatsPerMinute);
	}

	public void Stop()
	{
		IsStarted = false;
		BeatTimer.Stop(-BeatTimer.Calibration);
		VisualBeatTimer.Stop(-VisualBeatTimer.Calibration);
	}

	public override void _Process(double delta)
	{
		if (!IsFadingOut || CurrentTrack == null || !IsStarted)
			return;

		CurrentTrack.Volume -= Preferences.Singleton.MusicVolume * 0.5f * (float)delta;
	}

	private async void OnSceneTransitionStarted(PlayableScene scene)
	{
		IsFadingOut = true;
		BeatTimer.Stop(-BeatTimer.Calibration);
		VisualBeatTimer.Stop(-VisualBeatTimer.Calibration);
		await ToSignal(GetTree().CreateTimer(LongestCalibration / 1000), "timeout".ToStringName());
		CurrentTrack.Stop();
		CurrentTrack = null;
		IsStarted = false;

		PrepareSceneSong(scene);
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.SceneTransitionMusicReady);
	}

	private void OnSceneTransitionFinished(PlayableScene targetScene)
	{
		PlaySceneSong(targetScene);
	}

	public void SeekTo(double beatIndex)
	{
		StartingFromBeat = beatIndex;
	}

	public static float GetBeatsPerMinute() => Singleton.BeatsPerMinute;
	public static float GetBeatsPerSecond() => Singleton.BeatsPerSecond;
	public static float GetSecondsPerBeat() => Singleton.SecondsPerBeat;
	private static Music instance = null;
	public static Music Singleton
	{
		get => instance;
	}
}