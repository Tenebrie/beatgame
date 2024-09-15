using System;
using System.Collections.Generic;
using BeatGame.scripts.music;
using Godot;

namespace Project;

public partial class Music : Node
{
	[Signal] public delegate void BeatTickEventHandler(BeatTime beat);
	[Signal] public delegate void CurrentTrackChangedEventHandler(MusicTrack track);
	[Signal] public delegate void CurrentTrackPositionChangedEventHandler(double beatIndex);

	public class Settings
	{
		public float SongDelay = 2;
		public float StartingFromBeat = 0;
		public float TimingWindow = 0.05f; // seconds
		public float QueueingWindow = 30f; // seconds
		public const float MinBeatSize = 0.25f;
	}
	public readonly Settings settings = new();
	readonly MusicLibrary musicLibrary = new();

	public MusicTrack CurrentTrack;
	public AccurateTimer BeatTimer;
	public AccurateTimer VisualBeatTimer;

	public static float MinBeatSize => Settings.MinBeatSize;

	public double BeatIndex => PreciseBeatIndex * MinBeatSize;
	public float BeatsPerMinute => CurrentTrack != null ? CurrentTrack.BeatsPerMinute : 60;
	public float BeatsPerSecond => BeatsPerMinute / 60;
	public float SecondsPerBeat => 1 / BeatsPerSecond;
	public float GameSpeed => BeatsPerMinute / 60;

	public bool IsPlaying => IsStarted && PreciseBeatIndex > 0;
	public float TimingWindow => settings.TimingWindow;
	public float QueueingWindow => settings.QueueingWindow;

	bool IsStarted = false;
	bool IsFadingOut = false;
	public long PreciseBeatIndex = -1;

	public override void _EnterTree()
	{
		instance = this;
	}

	public override void _Ready()
	{
		InitPredictiveBeatCallbacks();
		InitAccurateTimers();

		AddChild(musicLibrary);

		LoadingManager.Singleton.SceneTransitionStarted += OnSceneTransitionStarted;
		LoadingManager.Singleton.SceneTransitionFinished += PlaySceneSong;

		var scene = Lib.Scene.ToEnumOrUnknown(GetTree().CurrentScene.SceneFilePath);
		PrepareSceneSong(scene);
		PlaySceneSong(scene);
	}

	void InitAccurateTimers()
	{
		BeatTimer = new AccurateTimer();
		AddChild(BeatTimer);

		VisualBeatTimer = new AccurateTimer();
		AddChild(VisualBeatTimer);

		CalibrateTimers(settings.SongDelay);

		BeatTimer.Timeout += OnInternalTimerTimeout;
		BeatTimer.CatchUpTick += () => PreciseBeatIndex += 1;
	}

	void CalibrateTimers(float delay)
	{
		BeatTimer.Calibration = 0;
		VisualBeatTimer.Calibration = delay;

		// Ensure no timer starts in the future
		var longestCalibration = Math.Max(BeatTimer.Calibration, VisualBeatTimer.Calibration);
		BeatTimer.Calibration -= longestCalibration;
		VisualBeatTimer.Calibration -= longestCalibration;
	}

	private void OnInternalTimerTimeout()
	{
		PreciseBeatIndex += 1;
		var beatTime = Utils.TickIndexToBeatTime(PreciseBeatIndex);
		try
		{
			EmitSignal(SignalName.BeatTick, beatTime.ToVariant());
		}
		catch (Exception ex) { GD.PrintErr(ex); }

		ProcessPredictiveBeatCallbacks();
	}

	private void PrepareSceneSong(PlayableScene scene)
	{
		if (scene is PlayableScene.MainMenu)
			settings.SongDelay = 0;
		else
			settings.SongDelay = 2;

		CalibrateTimers(settings.SongDelay);

		if (scene is PlayableScene.MainMenu)
			CurrentTrack = musicLibrary.MainMenu;
		else if (scene is PlayableScene.TrainingRoom)
			CurrentTrack = musicLibrary.TrainingRoom;
	}

	private async void PlaySceneSong(PlayableScene scene)
	{
		if (scene is PlayableScene.MainMenu or PlayableScene.TrainingRoom)
		{
			await ToSignal(GetTree().CreateTimer(1), "timeout".ToStringName());
			Start();
		}
	}

	public void Start(MusicTrack track)
	{
		CurrentTrack = track;
		Start();
	}

	public void Start()
	{
		if (IsStarted)
			return;

		var startTime = settings.StartingFromBeat * SecondsPerBeat;
		CurrentTrack.PlayAfterDelay(settings.SongDelay, startTime);
		CurrentTrack.Volume = 1.0f;

		PreciseBeatIndex = (long)Math.Round(settings.StartingFromBeat / MinBeatSize) - 1;
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

		CurrentTrack.Volume -= CurrentTrack.Volume * 4f * (float)delta;
	}

	private async void OnSceneTransitionStarted(PlayableScene scene)
	{
		IsFadingOut = true;
		BeatTimer.Stop(-BeatTimer.Calibration);
		VisualBeatTimer.Stop(-VisualBeatTimer.Calibration);
		await ToSignal(GetTree().CreateTimer(1), "timeout".ToStringName());
		CurrentTrack?.Stop();
		CurrentTrack = null;
		IsStarted = false;

		PrepareSceneSong(scene);
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.SceneTransitionMusicReady);
	}

	public void SeekTo(double beatIndex)
	{
		settings.StartingFromBeat = (float)beatIndex;
	}

	public static float GetBeatsPerMinute() => Singleton.BeatsPerMinute;
	public static float GetBeatsPerSecond() => Singleton.BeatsPerSecond;
	public static float GetSecondsPerBeat() => Singleton.SecondsPerBeat;

	private static Music instance = null;
	public static Music Singleton => instance;
}