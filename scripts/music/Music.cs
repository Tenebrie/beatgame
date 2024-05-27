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

	private MusicLibrary musicLibrary = new();

	public readonly long SongDelay = 3000; // ms

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

	private long LongestCalibration;
	public AccurateTimer BeatTimer;
	public AccurateTimer HalfBeatTimer;
	public AccurateTimer QuarterBeatTimer;
	public AccurateTimer VisualBeatTimer;

	public double BeatIndex = -1;
	private BeatTime BeatTimeState = BeatTime.Free;
	public MusicTrack CurrentTrack;

	public override void _EnterTree()
	{
		instance = this;
		AddChild(musicLibrary);

		BeatTimer = new AccurateTimer
		{
			Calibration = 0
		};
		AddChild(BeatTimer);

		HalfBeatTimer = new AccurateTimer
		{
			PrecedingTimer = BeatTimer,
			Calibration = 0
		};
		AddChild(HalfBeatTimer);

		QuarterBeatTimer = new AccurateTimer
		{
			PrecedingTimer = HalfBeatTimer,
			Calibration = 0
		};
		AddChild(QuarterBeatTimer);

		VisualBeatTimer = new AccurateTimer
		{
			Calibration = SongDelay
		};
		AddChild(VisualBeatTimer);

		// Ensure no timer starts in the future
		List<AccurateTimer> timers = new() { BeatTimer, HalfBeatTimer, QuarterBeatTimer, VisualBeatTimer };
		LongestCalibration = timers.OrderByDescending(timer => timer.Calibration).ToList()[0].Calibration;
		foreach (var timer in timers)
		{
			timer.Calibration -= LongestCalibration;
		}

		BeatTimer.BeatWindowUnlock += () =>
		{
			BeatTimeState |= BeatTime.One;
			EmitSignal(SignalName.BeatWindowUnlock, BeatTime.One.ToVariant());
		};
		BeatTimer.BeatWindowLock += () =>
		{
			BeatTimeState &= ~BeatTime.One;
			EmitSignal(SignalName.BeatWindowLock, BeatTime.One.ToVariant());
		};

		HalfBeatTimer.BeatWindowUnlock += () =>
		{
			if (IsTimeOpen(BeatTime.One))
				return;

			BeatTimeState |= BeatTime.Half;
			EmitSignal(SignalName.BeatWindowUnlock, BeatTime.Half.ToVariant());
		};
		HalfBeatTimer.BeatWindowLock += () =>
		{
			if (!IsTimeOpen(BeatTime.Half))
				return;

			BeatTimeState &= ~BeatTime.Half;
			EmitSignal(SignalName.BeatWindowLock, BeatTime.Half.ToVariant());
		};

		HalfBeatTimer.BeatWindowUnlock += () =>
		{
			if (IsTimeOpen(BeatTime.Half))
				return;

			BeatTimeState |= BeatTime.Quarter;
			EmitSignal(SignalName.BeatWindowUnlock, BeatTime.Quarter.ToVariant());
		};
		HalfBeatTimer.BeatWindowLock += () =>
		{
			if (!IsTimeOpen(BeatTime.Quarter))
				return;

			BeatTimeState &= ~BeatTime.Quarter;
			EmitSignal(SignalName.BeatWindowLock, BeatTime.Quarter.ToVariant());
		};

		BeatTimer.Timeout += () => OnInternalTimerTimeout(BeatTime.One);
		HalfBeatTimer.Timeout += () => OnInternalTimerTimeout(BeatTime.Half);
		QuarterBeatTimer.Timeout += () => OnInternalTimerTimeout(BeatTime.Quarter);

		SignalBus.Singleton.SceneTransitionStarted += OnSceneTransitionStarted;
		SignalBus.Singleton.SceneTransitionFinished += OnSceneTransitionFinished;
	}

	private void OnInternalTimerTimeout(BeatTime beat)
	{
		if (BeatIndex < 0)
			BeatIndex = 0;
		else
			BeatIndex += 0.25f;
		EmitSignal(SignalName.BeatTick, beat.ToVariant());
	}

	public bool IsTimeOpen(BeatTime time)
	{
		return (BeatTimeState & time) > 0;
	}

	public override void _Ready()
	{
		PlaySceneSong();
	}

	private void PlaySceneSong()
	{
		var scene = GetTree().CurrentScene.Name;
		CurrentTrack = scene == "BossArenaAeriel" ? musicLibrary.BossArenaAeriel : musicLibrary.TrainingRoom;
		Start();
	}

	public void Start()
	{
		CurrentTrack.PlayAfterDelay((float)SongDelay / 1000);

		BeatIndex = -1;
		IsStarted = true;
		BeatTimer.Start(BeatsPerMinute);
		HalfBeatTimer.Start(BeatsPerMinute * 2);
		QuarterBeatTimer.Start(BeatsPerMinute * 4);
		VisualBeatTimer.Start(BeatsPerMinute * 2);

		var boss = (TestBoss)GetTree().Root.FindChild("TestBoss", true, false);
		if (boss != null)
			AddChild(new TestBossTimeline(boss));
	}

	private async void OnSceneTransitionStarted(PackedScene _)
	{
		BeatTimer.Stop(-BeatTimer.Calibration);
		HalfBeatTimer.Stop(-HalfBeatTimer.Calibration);
		QuarterBeatTimer.Stop(-QuarterBeatTimer.Calibration);
		VisualBeatTimer.Stop(-VisualBeatTimer.Calibration);
		await ToSignal(GetTree().CreateTimer(LongestCalibration / 1000), "timeout");
		CurrentTrack.Stop();
		CurrentTrack.QueueFree();
		CurrentTrack = null;
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.SceneTransitionMusicReady);
	}

	private void OnSceneTransitionFinished(PackedScene _)
	{
		CurrentTrack = new MusicTrackSpaceship();
		AddChild(CurrentTrack);
		Start();
	}

	public double GetNearestBeatIndex()
	{
		return QuarterBeatTimer.GetNearestTickIndexAtEngineTime() / 4;
	}

	// Returns the time (in ms) to the nearest beat
	public long GetCurrentBeatOffset(BeatTime timings)
	{
		List<AccurateTimer> timers = new();
		if ((timings & BeatTime.One) > 0)
			timers.Add(BeatTimer);
		if ((timings & BeatTime.Half) > 0)
			timers.Add(HalfBeatTimer);
		if ((timings & BeatTime.Quarter) > 0)
			timers.Add(QuarterBeatTimer);

		return timers.Select(timer =>
		{
			var beatDuration = (long)(1f / BeatsPerMinute * 60 * 1000);
			var currentTime = (long)Time.Singleton.GetTicksMsec();
			var lastTickedAt = timer.LastTickedAt;
			if (currentTime - lastTickedAt < lastTickedAt + beatDuration - currentTime)
				return currentTime - lastTickedAt;
			else
				return -(lastTickedAt + beatDuration - currentTime);
		}).OrderBy(offset => Math.Abs(offset)).ToList()[0];
	}

	private static Music instance = null;
	public static Music Singleton
	{
		get => instance;
	}
}