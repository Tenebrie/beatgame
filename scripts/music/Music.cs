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

	private long LongestCalibration;
	public AccurateTimer FourBeatTimer;
	public AccurateTimer TwoBeatTimer;
	public AccurateTimer BeatTimer;
	public AccurateTimer HalfBeatTimer;
	public AccurateTimer QuarterBeatTimer;
	public AccurateTimer VisualBeatTimer;

	public long InternalBeatIndex = -1;
	public double BeatIndex
	{
		get
		{
			this.Log(InternalBeatIndex * MinBeatSize);
			return InternalBeatIndex * MinBeatSize;
		}
	}
	private BeatTime BeatTimeState = BeatTime.Free;
	public MusicTrack CurrentTrack;

	public override void _EnterTree()
	{
		instance = this;
		AddChild(musicLibrary);

		FourBeatTimer = new AccurateTimer
		{
			Calibration = 0,
		};
		AddChild(FourBeatTimer);

		TwoBeatTimer = new AccurateTimer
		{
			PrecedingTimer = FourBeatTimer,
			Calibration = 0,
		};
		AddChild(TwoBeatTimer);

		BeatTimer = new AccurateTimer
		{
			PrecedingTimer = TwoBeatTimer,
			Calibration = 0,
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
		List<AccurateTimer> timers = new() { FourBeatTimer, TwoBeatTimer, BeatTimer, HalfBeatTimer, QuarterBeatTimer, VisualBeatTimer };
		LongestCalibration = timers.OrderByDescending(timer => timer.Calibration).ToList()[0].Calibration;
		foreach (var timer in timers)
			timer.Calibration -= LongestCalibration;

		FourBeatTimer.BeatWindowUnlock += () =>
		{
			BeatTimeState |= BeatTime.Whole;
			EmitSignal(SignalName.BeatWindowUnlock, BeatTime.Whole.ToVariant());
		};
		FourBeatTimer.BeatWindowLock += () =>
		{
			BeatTimeState &= ~BeatTime.Whole;
			EmitSignal(SignalName.BeatWindowLock, BeatTime.Whole.ToVariant());
		};

		TwoBeatTimer.BeatWindowUnlock += () =>
		{
			if (IsTimeOpen(BeatTime.Whole))
				return;

			BeatTimeState |= BeatTime.Half;
			EmitSignal(SignalName.BeatWindowUnlock, BeatTime.Half.ToVariant());
		};
		TwoBeatTimer.BeatWindowLock += () =>
		{
			if (!IsTimeOpen(BeatTime.Half))
				return;

			BeatTimeState &= ~BeatTime.Half;
			EmitSignal(SignalName.BeatWindowLock, BeatTime.Half.ToVariant());
		};

		BeatTimer.BeatWindowUnlock += () =>
		{
			if (IsTimeOpen(BeatTime.Half))
				return;

			BeatTimeState |= BeatTime.Quarter;
			EmitSignal(SignalName.BeatWindowUnlock, BeatTime.Quarter.ToVariant());
		};
		BeatTimer.BeatWindowLock += () =>
		{
			if (!IsTimeOpen(BeatTime.Quarter))
				return;

			BeatTimeState &= ~BeatTime.Quarter;
			EmitSignal(SignalName.BeatWindowLock, BeatTime.Quarter.ToVariant());
		};

		HalfBeatTimer.BeatWindowUnlock += () =>
		{
			if (IsTimeOpen(BeatTime.Quarter))
				return;

			BeatTimeState |= BeatTime.Eighth;
			EmitSignal(SignalName.BeatWindowUnlock, BeatTime.Eighth.ToVariant());
		};
		HalfBeatTimer.BeatWindowLock += () =>
		{
			if (!IsTimeOpen(BeatTime.Eighth))
				return;

			BeatTimeState &= ~BeatTime.Eighth;
			EmitSignal(SignalName.BeatWindowLock, BeatTime.Eighth.ToVariant());
		};

		QuarterBeatTimer.BeatWindowUnlock += () =>
		{
			if (IsTimeOpen(BeatTime.Eighth))
				return;

			BeatTimeState |= BeatTime.Sixteenth;
			EmitSignal(SignalName.BeatWindowUnlock, BeatTime.Sixteenth.ToVariant());
		};
		QuarterBeatTimer.BeatWindowLock += () =>
		{
			if (!IsTimeOpen(BeatTime.Sixteenth))
				return;

			BeatTimeState &= ~BeatTime.Sixteenth;
			EmitSignal(SignalName.BeatWindowLock, BeatTime.Sixteenth.ToVariant());
		};

		FourBeatTimer.Timeout += () => OnInternalTimerTimeout(BeatTime.Whole);
		TwoBeatTimer.Timeout += () => OnInternalTimerTimeout(BeatTime.Half);
		BeatTimer.Timeout += () => OnInternalTimerTimeout(BeatTime.Quarter);
		HalfBeatTimer.Timeout += () => OnInternalTimerTimeout(BeatTime.Eighth);
		QuarterBeatTimer.Timeout += () => OnInternalTimerTimeout(BeatTime.Sixteenth);

		SignalBus.Singleton.SceneTransitionStarted += OnSceneTransitionStarted;
		SignalBus.Singleton.SceneTransitionFinished += OnSceneTransitionFinished;
	}

	private void OnInternalTimerTimeout(BeatTime beat)
	{
		InternalBeatIndex += 1;
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

		InternalBeatIndex = (long)Math.Round(StartingFromBeat / MinBeatSize) - 1;
		IsStarted = true;
		FourBeatTimer.Start(BeatsPerMinute / 4);
		TwoBeatTimer.Start(BeatsPerMinute / 2);
		BeatTimer.Start(BeatsPerMinute);
		HalfBeatTimer.Start(BeatsPerMinute * 2);
		QuarterBeatTimer.Start(BeatsPerMinute * 4);
		VisualBeatTimer.Start(BeatsPerMinute);
	}

	private async void OnSceneTransitionStarted(PackedScene _)
	{
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

	public double GetNearestBeatIndex()
	{
		return QuarterBeatTimer.GetNearestTickIndexAtEngineTime() / 4;
	}

	// Returns the time (in ms) to the nearest beat
	public long GetCurrentBeatOffset(BeatTime timings)
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

	public void SeekTo(double beatIndex)
	{
		StartingFromBeat = beatIndex;
		if (IsStarted)
		{
			FourBeatTimer.SeekTo(beatIndex);
			TwoBeatTimer.SeekTo(beatIndex);
			BeatTimer.SeekTo(beatIndex);
			HalfBeatTimer.SeekTo(beatIndex);
			QuarterBeatTimer.SeekTo(beatIndex);
			VisualBeatTimer.SeekTo(beatIndex);
		}
		EmitSignal(SignalName.CurrentTrackPositionChanged, beatIndex);
	}

	private static Music instance = null;
	public static Music Singleton
	{
		get => instance;
	}
}