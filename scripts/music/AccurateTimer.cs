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

	public long Calibration = 0;
	public const long TimingWindow = 100; // ms before and after beat

	private long startTime;
	private long waitTime;
	public long LastTickedAt;
	private bool lockedState = true;
	private long beatCount = -1;

	public void Start(float bpm)
	{
		waitTime = (long)Math.Floor(1 / bpm * 60 * 1000);
		startTime = (long)Time.Singleton.GetTicksMsec();
		LastTickedAt = startTime;
	}

	public override void _Process(double delta)
	{
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

		var tickIndex = GetTickIndexAt(songTime);
		if (tickIndex > beatCount)
		{
			beatCount = tickIndex;
			LastTickedAt = time;
			EmitSignal(SignalName.Timeout);
		}
	}

	public long GetTickIndexAt(long time)
	{
		return time / waitTime;
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
}