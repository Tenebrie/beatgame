using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace Project;

public partial class Music : Node
{
	public readonly long SongDelay = 3000; // ms

	public float BeatsPerMinute
	{
		get => CurrentTrack.BeatsPerMinute;
	}
	public bool IsStarted = false;

	public AccurateTimer BeatTimer;
	public AccurateTimer VisualBeatTimer;

	private BeatTime BeatTimeState = BeatTime.Free;
	private MusicTrack CurrentTrack;

	public override void _EnterTree()
	{
		instance = this;

		BeatTimer = new AccurateTimer();
		AddChild(BeatTimer);
		BeatTimer.Calibration = 0;
		VisualBeatTimer = new AccurateTimer();
		AddChild(VisualBeatTimer);
		VisualBeatTimer.Calibration = SongDelay;

		CurrentTrack = new MusicTrackSpaceship();
		AddChild(CurrentTrack);

		// Ensure no timer starts in the future
		List<AccurateTimer> timers = new() { BeatTimer, VisualBeatTimer };
		long longestCalibration = timers.OrderByDescending(timer => timer.Calibration).ToList()[0].Calibration;
		foreach (var timer in timers)
		{
			timer.Calibration -= longestCalibration;
		}

		BeatTimer.BeatWindowUnlock += () => BeatTimeState |= BeatTime.One;
		BeatTimer.BeatWindowLock += () => BeatTimeState &= ~BeatTime.One;
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
		VisualBeatTimer.Start(BeatsPerMinute);

		var boss = (TestBoss)GetTree().Root.FindChild("TestBoss", true, false);
		AddChild(new TestBossTimeline(boss));
	}

	public long GetNearestBeatIndex()
	{
		return BeatTimer.GetTickIndexAtEngineTime();
	}

	// Returns the time (in ms) to the nearest beat
	public long GetCurrentBeatOffset(BeatTime beatTime)
	{
		if (beatTime != BeatTime.One)
			throw new NotImplementedException();

		var beatDuration = (long)(1f / BeatsPerMinute * 60 * 1000);
		var currentTime = (long)Time.Singleton.GetTicksMsec();
		var lastTickedAt = BeatTimer.LastTickedAt;
		if (currentTime - lastTickedAt < lastTickedAt + beatDuration - currentTime)
			return currentTime - lastTickedAt;
		else
			return -(lastTickedAt + beatDuration - currentTime);
	}

	private static Music instance = null;
	public static Music Singleton
	{
		get => instance;
	}
}