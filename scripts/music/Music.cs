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
	public const double MinBeatSize = 0.25;
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
	public AccurateTimer FourBeatTimer;
	public AccurateTimer TwoBeatTimer;
	public AccurateTimer BeatTimer;
	public AccurateTimer HalfBeatTimer;
	public AccurateTimer QuarterBeatTimer;
	public AccurateTimer VisualBeatTimer;

	public long PreciseBeatIndex = -1;
	public double BeatIndex { get => PreciseBeatIndex * MinBeatSize; }
	public long SongTime { get => BeatTimer.GetSongTime(); }
	public long TimingWindow { get => AccurateTimer.TimingWindow; }
	private BeatTime BeatTimeState = BeatTime.Free;
	public MusicTrack CurrentTrack;

	public override void _EnterTree()
	{
		instance = this;
		AddChild(musicLibrary);

		FourBeatTimer = new AccurateTimer
		{
			Calibration = 0,
			BeatTime = BeatTime.Whole,
		};
		AddChild(FourBeatTimer);

		TwoBeatTimer = new AccurateTimer
		{
			PrecedingTimer = FourBeatTimer,
			Calibration = 0,
			BeatTime = BeatTime.Half,
		};
		AddChild(TwoBeatTimer);

		BeatTimer = new AccurateTimer
		{
			PrecedingTimer = TwoBeatTimer,
			Calibration = 0,
			BeatTime = BeatTime.Quarter,
		};
		AddChild(BeatTimer);

		HalfBeatTimer = new AccurateTimer
		{
			PrecedingTimer = BeatTimer,
			Calibration = 0,
			BeatTime = BeatTime.Eighth,
		};
		AddChild(HalfBeatTimer);

		QuarterBeatTimer = new AccurateTimer
		{
			PrecedingTimer = HalfBeatTimer,
			Calibration = 0,
			BeatTime = BeatTime.Sixteenth,
		};
		AddChild(QuarterBeatTimer);

		VisualBeatTimer = new AccurateTimer
		{
			Calibration = SongDelay,
			BeatTime = BeatTime.Quarter,
		};
		AddChild(VisualBeatTimer);

		// Ensure no timer starts in the future
		List<AccurateTimer> timers = new() { FourBeatTimer, TwoBeatTimer, BeatTimer, HalfBeatTimer, QuarterBeatTimer, VisualBeatTimer };
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

		FourBeatTimer.BeatWindowUnlock += OnWindowUnlock;
		FourBeatTimer.BeatWindowLock += OnWindowLock;

		TwoBeatTimer.BeatWindowUnlock += OnWindowUnlock;
		TwoBeatTimer.BeatWindowLock += OnWindowLock;

		BeatTimer.BeatWindowUnlock += OnWindowUnlock;
		BeatTimer.BeatWindowLock += OnWindowLock;

		HalfBeatTimer.BeatWindowUnlock += OnWindowUnlock;
		HalfBeatTimer.BeatWindowLock += OnWindowLock;

		QuarterBeatTimer.BeatWindowUnlock += OnWindowUnlock;
		QuarterBeatTimer.BeatWindowLock += OnWindowLock;

		FourBeatTimer.Timeout += OnInternalTimerTimeout;
		TwoBeatTimer.Timeout += OnInternalTimerTimeout;
		BeatTimer.Timeout += OnInternalTimerTimeout;
		HalfBeatTimer.Timeout += OnInternalTimerTimeout;
		QuarterBeatTimer.Timeout += OnInternalTimerTimeout;

		// If the timer skipped a number of ticks, this will be called
		FourBeatTimer.CatchupTick += OnInternalTimerCatchupTick;
		TwoBeatTimer.CatchupTick += OnInternalTimerCatchupTick;
		BeatTimer.CatchupTick += OnInternalTimerCatchupTick;
		HalfBeatTimer.CatchupTick += OnInternalTimerCatchupTick;
		QuarterBeatTimer.CatchupTick += OnInternalTimerCatchupTick;

		SignalBus.Singleton.SceneTransitionStarted += OnSceneTransitionStarted;
		SignalBus.Singleton.SceneTransitionFinished += OnSceneTransitionFinished;
	}

	private void OnInternalTimerTimeout(BeatTime beat)
	{
		PreciseBeatIndex += 1;
		EmitSignal(SignalName.BeatTick, beat.ToVariant());
	}

	private void OnInternalTimerCatchupTick(BeatTime beat)
	{
		PreciseBeatIndex += 1;
	}

	public bool IsTimeOpen(BeatTime time)
	{
		return (BeatTimeState & time) > 0;
	}

	public override void _Ready()
	{
		PlaySceneSong();
	}

	private async void PlaySceneSong()
	{
		var scene = GetTree().CurrentScene.Name;
		CurrentTrack = scene == "BossArenaAeriel" ? musicLibrary.BossArenaAeriel : musicLibrary.TrainingRoom;
		if (scene == "TrainingRoom")
		{
			await ToSignal(GetTree().CreateTimer(1), "timeout");
			Start();
		}
	}

	public void Start()
	{
		if (IsStarted)
			return;

		var bosses = BaseUnit.AllUnits.Where(unit => unit is BossAeriel).Cast<BossAeriel>().ToList();
		if (bosses.Count > 0)
			AddChild(new TestBossTimeline(bosses[0]));

		var startTime = (float)StartingFromBeat * SecondsPerBeat;
		CurrentTrack.PlayAfterDelay((float)SongDelay / 1000, startTime);
		CurrentTrack.Volume = Preferences.Singleton.MainVolume;

		PreciseBeatIndex = (long)Math.Round(StartingFromBeat / MinBeatSize) - 1;
		IsStarted = true;
		IsFadingOut = false;
		FourBeatTimer.Start(BeatsPerMinute, 0.25f);
		TwoBeatTimer.Start(BeatsPerMinute, 0.5f);
		BeatTimer.Start(BeatsPerMinute, 1);
		HalfBeatTimer.Start(BeatsPerMinute, 2);
		QuarterBeatTimer.Start(BeatsPerMinute, 4);
		VisualBeatTimer.Start(BeatsPerMinute, 1);
	}

	public override void _Process(double delta)
	{
		if (!IsFadingOut || CurrentTrack == null)
			return;

		CurrentTrack.Volume -= Preferences.Singleton.MainVolume * 0.5f * (float)delta;
	}

	private async void OnSceneTransitionStarted(PackedScene _)
	{
		IsFadingOut = true;
		FourBeatTimer.Stop(-FourBeatTimer.Calibration);
		TwoBeatTimer.Stop(-TwoBeatTimer.Calibration);
		BeatTimer.Stop(-BeatTimer.Calibration);
		HalfBeatTimer.Stop(-HalfBeatTimer.Calibration);
		QuarterBeatTimer.Stop(-QuarterBeatTimer.Calibration);
		VisualBeatTimer.Stop(-VisualBeatTimer.Calibration);
		await ToSignal(GetTree().CreateTimer(LongestCalibration / 1000), "timeout");
		CurrentTrack.Stop();
		CurrentTrack.QueueFree();
		CurrentTrack = null;
		IsStarted = false;
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.SceneTransitionMusicReady);
	}

	private void OnSceneTransitionFinished(PackedScene _)
	{
		PlaySceneSong();
	}

	public double GetNearestBeatIndex(BeatTime timings)
	{
		List<AccurateTimer> timers = new();
		if ((timings & BeatTime.Whole) > 0)
			timers.Add(FourBeatTimer);
		if ((timings & BeatTime.Half) > 0)
			timers.Add(TwoBeatTimer);
		if ((timings & BeatTime.Quarter) > 0)
			timers.Add(BeatTimer);
		if ((timings & BeatTime.Eighth) > 0)
			timers.Add(HalfBeatTimer);
		if ((timings & BeatTime.Sixteenth) > 0)
			timers.Add(QuarterBeatTimer);

		if (timers.Count == 0)
			return 0;

		var beatDuration = (long)(1f / BeatsPerMinute * 60 * 1000);
		var currentTime = (long)Time.Singleton.GetTicksMsec();
		var closestTimer = timers.OrderBy(timer =>
		{
			var lastTickedAt = timer.LastTickedAt;
			return Math.Abs(currentTime - lastTickedAt);
		}).ToList()[0];

		return closestTimer.GetNearestTickIndexAtEngineTime() / closestTimer.SpeedFactor;
	}

	// Returns the time (in ms) to the nearest beat
	public long GetCurrentBeatOffset(BeatTime timings)
	{
		if ((timings & BeatTime.Free) > 0)
			return 0;

		List<AccurateTimer> timers = new();
		if ((timings & BeatTime.Whole) > 0)
			timers.Add(FourBeatTimer);
		if ((timings & BeatTime.Half) > 0)
			timers.Add(TwoBeatTimer);
		if ((timings & BeatTime.Quarter) > 0)
			timers.Add(BeatTimer);
		if ((timings & BeatTime.Eighth) > 0)
			timers.Add(HalfBeatTimer);
		if ((timings & BeatTime.Sixteenth) > 0)
			timers.Add(QuarterBeatTimer);

		var beatDuration = (long)(1f / BeatsPerMinute * 60 * 1000);
		var currentTime = (long)Time.Singleton.GetTicksMsec();
		return timers.Select(timer =>
		{
			var lastTickedAt = timer.LastTickedAt;
			if (currentTime - lastTickedAt < lastTickedAt + beatDuration - currentTime)
				return currentTime - lastTickedAt;
			else
				return -(lastTickedAt + beatDuration - currentTime);
		}).OrderBy(offset => Math.Abs(offset)).ToList()[0];
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