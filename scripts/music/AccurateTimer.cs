using System;
using System.Diagnostics;
using Godot;

namespace Project;

public partial class AccurateTimer : Node
{
	[Signal]
	public delegate void TimeoutEventHandler();
	[Signal]
	public delegate void BeatWindowUnlockEventHandler();
	[Signal]
	public delegate void BeatWindowLockEventHandler();

	public const long TimingWindow = 100; // ms before and after beat
	public long Calibration = 0;
	public AccurateTimer PrecedingTimer;

	private long startTime;
	private long waitTime;
	public long LastTickedAt;
	private bool lockedState = true;
	private long internalTickIndex = -1;
	public long TickIndex = -1;

	public void Start(float bpm)
	{
		waitTime = (long)Math.Floor(1 / bpm * 60 * 1000);
		startTime = (long)Time.Singleton.GetTicksMsec();
		LastTickedAt = startTime;
	}

	public override void _Process(double delta)
	{
		if (!Music.Singleton.IsStarted)
			return;

		var time = (long)Time.Singleton.GetTicksMsec();
		var songTime = time - startTime + Calibration;

		if (songTime < 0)
			return;

		var locked = IsLockedAt(songTime);
		if (lockedState && !locked)
		{
			lockedState = false;
			EmitSignal(SignalName.BeatWindowUnlock);
		}
		else if (!lockedState && locked)
		{
			lockedState = true;
			EmitSignal(SignalName.BeatWindowLock);
		}

		var tickIndex = GetTickIndexAtSongTime(songTime);
		if (tickIndex > internalTickIndex)
		{
			internalTickIndex = tickIndex;
			if (PrecedingTimer == null || PrecedingTimer.IsUnlockedNow())
			{
				TickIndex += 1;
				var name = this == Music.Singleton.BeatTimer ? "Beat" : this == Music.Singleton.HalfBeatTimer ? "Half" : "Visual";
				LastTickedAt = time;
				EmitSignal(SignalName.Timeout);
			}
		}
	}

	public long GetTickIndexAtSongTime(long time)
	{
		if (waitTime == 0)
			return 0;

		return time / waitTime;
	}

	public double GetTickIndexAtEngineTime()
	{
		if (waitTime == 0)
			return 0;

		var time = (long)Time.Singleton.GetTicksMsec();
		var songTime = time - startTime + Calibration;
		return Math.Round((double)songTime / waitTime);
	}

	private bool IsLockedAt(long time)
	{
		var remainder = time % waitTime;
		if (remainder < TimingWindow || remainder > (waitTime - TimingWindow))
		{
			return false;
		}
		return true;
	}

	public bool IsUnlockedNow()
	{
		return lockedState;
	}
}