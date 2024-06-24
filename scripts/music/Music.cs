using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Project;

public partial class Music : Node
{
	[Signal]
	public delegate void BeatTickEventHandler(BeatTime beat);
	[Signal]
	public delegate void BeatWindowUnlockEventHandler(BeatTime beat);
	[Signal]
	public delegate void BeatWindowLockEventHandler(BeatTime beat);
	[Signal]
	public delegate void CurrentTrackPositionChangedEventHandler(double beatIndex);

	private MusicLibrary musicLibrary = new();

	public readonly long SongDelay = 2000; // ms
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
	public bool IsStarted = false;
	bool IsFadingOut = false;

	private long LongestCalibration;
	public AccurateTimer WholeNoteTimer;
	public AccurateTimer HalfNoteTimer;
	public AccurateTimer QuarterNoteTimer;
	public AccurateTimer EigthNoteTimer;
	public AccurateTimer SixteenthNoteTimer;
	public AccurateTimer VisualBeatTimer;

	public long PreciseBeatIndex = -1;
	public double BeatIndex { get => PreciseBeatIndex * MinBeatSize; }
	public long SongTime { get => WholeNoteTimer.GetSongTime() + (long)(StartingFromBeat * SecondsPerBeat * 1000); }
	public float TimingWindow { get => AccurateTimer.TimingWindow; }
	public float TimingWindowMs { get => AccurateTimer.TimingWindow * 1000f; }
	private BeatTime BeatTimeState = BeatTime.Free;
	public MusicTrack CurrentTrack;

	public override void _EnterTree()
	{
		instance = this;
		AddChild(musicLibrary);

		WholeNoteTimer = new AccurateTimer
		{
			Calibration = 0,
			BeatTime = BeatTime.Whole,
		};
		AddChild(WholeNoteTimer);

		HalfNoteTimer = new AccurateTimer
		{
			Calibration = 0,
			BeatTime = BeatTime.Half,
		};
		AddChild(HalfNoteTimer);

		QuarterNoteTimer = new AccurateTimer
		{
			Calibration = 0,
			BeatTime = BeatTime.Quarter,
		};
		AddChild(QuarterNoteTimer);

		EigthNoteTimer = new AccurateTimer
		{
			Calibration = 0,
			BeatTime = BeatTime.Eighth,
		};
		AddChild(EigthNoteTimer);

		SixteenthNoteTimer = new AccurateTimer
		{
			Calibration = 0,
			BeatTime = BeatTime.Sixteenth,
		};
		AddChild(SixteenthNoteTimer);

		VisualBeatTimer = new AccurateTimer
		{
			Calibration = SongDelay,
			BeatTime = BeatTime.Quarter,
		};
		AddChild(VisualBeatTimer);

		// Ensure no timer starts in the future
		List<AccurateTimer> timers = new() { WholeNoteTimer, HalfNoteTimer, QuarterNoteTimer, EigthNoteTimer, SixteenthNoteTimer, VisualBeatTimer };
		LongestCalibration = timers.OrderByDescending(timer => timer.Calibration).ToList()[0].Calibration;
		foreach (var timer in timers)
			timer.Calibration -= LongestCalibration;

		void OnWindowUnlock(BeatTime time)
		{
			BeatTimeState |= time;
			EmitSignal(SignalName.BeatWindowUnlock, time.ToVariant());
		}
		void OnWindowLock(BeatTime time)
		{
			BeatTimeState &= ~time;
			EmitSignal(SignalName.BeatWindowLock, time.ToVariant());
		}

		WholeNoteTimer.BeatWindowUnlock += OnWindowUnlock;
		WholeNoteTimer.BeatWindowLock += OnWindowLock;

		HalfNoteTimer.BeatWindowUnlock += OnWindowUnlock;
		HalfNoteTimer.BeatWindowLock += OnWindowLock;

		QuarterNoteTimer.BeatWindowUnlock += OnWindowUnlock;
		QuarterNoteTimer.BeatWindowLock += OnWindowLock;

		EigthNoteTimer.BeatWindowUnlock += OnWindowUnlock;
		EigthNoteTimer.BeatWindowLock += OnWindowLock;

		SixteenthNoteTimer.BeatWindowUnlock += OnWindowUnlock;
		SixteenthNoteTimer.BeatWindowLock += OnWindowLock;

		WholeNoteTimer.Timeout += OnInternalTimerTimeout;
		HalfNoteTimer.Timeout += OnInternalTimerTimeout;
		QuarterNoteTimer.Timeout += OnInternalTimerTimeout;
		EigthNoteTimer.Timeout += OnInternalTimerTimeout;
		SixteenthNoteTimer.Timeout += OnInternalTimerTimeout;

		WholeNoteTimer.CatchUpTick += OnInternalTimerCatchUpTick;
		HalfNoteTimer.CatchUpTick += OnInternalTimerCatchUpTick;
		QuarterNoteTimer.CatchUpTick += OnInternalTimerCatchUpTick;
		EigthNoteTimer.CatchUpTick += OnInternalTimerCatchUpTick;
		SixteenthNoteTimer.CatchUpTick += OnInternalTimerCatchUpTick;

		SignalBus.Singleton.SceneTransitionStarted += OnSceneTransitionStarted;
		SignalBus.Singleton.SceneTransitionFinished += OnSceneTransitionFinished;
	}

	private void OnInternalTimerTimeout(BeatTime beat)
	{
		PreciseBeatIndex += 1;
		EmitSignal(SignalName.BeatTick, beat.ToVariant());
	}

	private void OnInternalTimerCatchUpTick(BeatTime beat)
	{
		PreciseBeatIndex += 1;
	}

	public bool IsTimeOpen(BeatTime time)
	{
		return (BeatTimeState & time) > 0;
	}

	public override void _Ready()
	{
		var scene = GetTree().CurrentScene.SceneFilePath;
		PrepareSceneSong(scene);
		PlaySceneSong(scene);
	}

	private void PrepareSceneSong(string scenePath)
	{
		CurrentTrack = Lib.Scene.Is(PlayableScene.BossArenaAeriel, scenePath) ? musicLibrary.BossArenaAeriel : musicLibrary.TrainingRoom;
	}

	private async void PlaySceneSong(string scenePath)
	{
		if (Lib.Scene.Is(PlayableScene.TrainingRoom, scenePath))
		{
			await ToSignal(GetTree().CreateTimer(1), "timeout");
			Start();
		}
	}

	public void Start()
	{
		if (IsStarted)
			return;

		var startTime = (float)StartingFromBeat * SecondsPerBeat;
		CurrentTrack.PlayAfterDelay((float)SongDelay / 1000, startTime);
		CurrentTrack.Volume = Preferences.Singleton.MainVolume;

		PreciseBeatIndex = (long)Math.Round(StartingFromBeat / MinBeatSize) - 1;
		IsStarted = true;
		IsFadingOut = false;

		WholeNoteTimer.Start(BeatsPerMinute, speedModifier: 0.25f, 0);
		HalfNoteTimer.Start(BeatsPerMinute, speedModifier: 0.25f, 1);
		QuarterNoteTimer.Start(BeatsPerMinute, speedModifier: 0.5f, 1);
		EigthNoteTimer.Start(BeatsPerMinute, speedModifier: 1f, 1);
		SixteenthNoteTimer.Start(BeatsPerMinute, speedModifier: 2f, 1);

		VisualBeatTimer.Start(BeatsPerMinute, speedModifier: 1f, 0);
	}

	public override void _Process(double delta)
	{
		if (!IsFadingOut || CurrentTrack == null || !IsStarted)
			return;

		CurrentTrack.Volume -= Preferences.Singleton.MainVolume * 0.5f * (float)delta;
	}

	private async void OnSceneTransitionStarted(PackedScene targetScene)
	{
		IsFadingOut = true;
		WholeNoteTimer.Stop(-WholeNoteTimer.Calibration);
		HalfNoteTimer.Stop(-HalfNoteTimer.Calibration);
		QuarterNoteTimer.Stop(-QuarterNoteTimer.Calibration);
		EigthNoteTimer.Stop(-EigthNoteTimer.Calibration);
		SixteenthNoteTimer.Stop(-SixteenthNoteTimer.Calibration);
		VisualBeatTimer.Stop(-VisualBeatTimer.Calibration);
		await ToSignal(GetTree().CreateTimer(LongestCalibration / 1000), "timeout");
		CurrentTrack.Stop();
		CurrentTrack = null;
		IsStarted = false;

		PrepareSceneSong(targetScene.ResourcePath);
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.SceneTransitionMusicReady);
	}

	private void OnSceneTransitionFinished(PackedScene targetScene)
	{
		PlaySceneSong(targetScene.ResourcePath);
	}

	public double GetNearestBeatIndex(BeatTime timings)
	{
		if (timings == BeatTime.Free)
			timings = BeatTime.All;

		List<AccurateTimer> timers = new();
		if ((timings & BeatTime.Whole) > 0)
			timers.Add(WholeNoteTimer);
		if ((timings & BeatTime.Half) > 0)
			timers.Add(HalfNoteTimer);
		if ((timings & BeatTime.Quarter) > 0)
			timers.Add(QuarterNoteTimer);
		if ((timings & BeatTime.Eighth) > 0)
			timers.Add(EigthNoteTimer);
		if ((timings & BeatTime.Sixteenth) > 0)
			timers.Add(SixteenthNoteTimer);

		if (timers.Count == 0)
			return BeatIndex;

		var beatDuration = (long)(1f / BeatsPerMinute * 60 * 1000);
		var currentTime = (long)Time.Singleton.GetTicksMsec();
		var closestTimer = timers.OrderBy(timer => Math.Abs(BeatIndex - timer.GetNearestBeatIndex())).ToList()[0];

		return closestTimer.GetNearestBeatIndex();
	}

	// Returns the time (in ms) to the nearest beat
	public long GetCurrentBeatOffset(BeatTime timings)
	{
		if (timings == BeatTime.Free)
			timings = BeatTime.All;

		List<AccurateTimer> timers = new();
		if ((timings & BeatTime.Whole) > 0)
			timers.Add(WholeNoteTimer);
		if ((timings & BeatTime.Half) > 0)
			timers.Add(HalfNoteTimer);
		if ((timings & BeatTime.Quarter) > 0)
			timers.Add(QuarterNoteTimer);
		if ((timings & BeatTime.Eighth) > 0)
			timers.Add(EigthNoteTimer);
		if ((timings & BeatTime.Sixteenth) > 0)
			timers.Add(SixteenthNoteTimer);

		var beatDuration = (long)(1f / BeatsPerMinute * 60 * 1000);
		var currentTime = (long)Time.Singleton.GetTicksMsec();
		var closestTimer = timers.OrderBy(timer => Math.Abs(BeatIndex - timer.GetNearestBeatIndex())).ToList()[0];

		return closestTimer.GetTimeToNearestTick();
	}

	public void SeekTo(double beatIndex)
	{
		StartingFromBeat = beatIndex;
		EmitSignal(SignalName.CurrentTrackPositionChanged, beatIndex);
	}

	private static Music instance = null;
	public static Music Singleton
	{
		get => instance;
	}
}