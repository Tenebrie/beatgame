using System;
using System.Collections.Generic;
using System.Diagnostics;
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

	public readonly long SongDelay = 3000; // ms

	public float BeatsPerMinute
	{
		get => CurrentTrack.BeatsPerMinute;
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

	public AccurateTimer BeatTimer;
	public AccurateTimer HalfBeatTimer;
	public AccurateTimer VisualBeatTimer;

	public long BeatIndex = -1;
	private BeatTime BeatTimeState = BeatTime.Free;
	public MusicTrack CurrentTrack;

	public override void _EnterTree()
	{
		instance = this;

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

		VisualBeatTimer = new AccurateTimer
		{
			Calibration = SongDelay
		};
		AddChild(VisualBeatTimer);

		// CurrentTrack = new MusicTrackTest();
		CurrentTrack = new MusicTrackSpaceship();
		AddChild(CurrentTrack);

		// Ensure no timer starts in the future
		List<AccurateTimer> timers = new() { BeatTimer, HalfBeatTimer, VisualBeatTimer };
		long longestCalibration = timers.OrderByDescending(timer => timer.Calibration).ToList()[0].Calibration;
		foreach (var timer in timers)
		{
			timer.Calibration -= longestCalibration;
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

		BeatTimer.Timeout += () => OnInternalTimerTimeout(BeatTime.One);
		HalfBeatTimer.Timeout += () => OnInternalTimerTimeout(BeatTime.Half);
	}

	private void OnInternalTimerTimeout(BeatTime beat)
	{
		BeatIndex += 1;
		EmitSignal(SignalName.BeatTick, beat.ToVariant());
	}

	public bool IsTimeOpen(BeatTime time)
	{
		return (BeatTimeState & time) > 0;
	}

	public override void _Ready()
	{
		Start();
	}

	public async void Start()
	{
		await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
		CurrentTrack.PlayAfterDelay((float)SongDelay / 1000);

		IsStarted = true;
		BeatTimer.Start(BeatsPerMinute);
		HalfBeatTimer.Start(BeatsPerMinute * 2);
		VisualBeatTimer.Start(BeatsPerMinute * 2);

		var boss = (TestBoss)GetTree().Root.FindChild("TestBoss", true, false);
		AddChild(new TestBossTimeline(boss));
	}

	public double GetNearestBeatIndex()
	{
		return BeatTimer.GetTickIndexAtEngineTime() + ((HalfBeatTimer.GetTickIndexAtEngineTime() - (BeatTimer.GetTickIndexAtEngineTime() * 2)) / 2);
	}

	// Returns the time (in ms) to the nearest beat
	public long GetCurrentBeatOffset()
	{
		List<AccurateTimer> timers = new() { BeatTimer, HalfBeatTimer };

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