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

	private long startTime;
	private long waitTime;
	private long lastTickedAt;
	private bool lockedState = true;
	private long beatCount = -1;

	private long timingWindow = 125; // ms before and after beat

	public void Start(float bpm)
	{
		waitTime = (long)Math.Floor(1 / bpm * 60 * 1000);
		startTime = (long)Time.Singleton.GetTicksMsec();
		lastTickedAt = startTime;
	}

	public override void _Process(double delta)
	{
		var songTime = (long)Time.Singleton.GetTicksMsec() - startTime + Calibration;

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
			EmitSignal(SignalName.Timeout);
		}
	}

	private long GetTickIndexAt(long time)
	{
		return time / waitTime;
	}

	private bool IsLockedAt(long time)
	{
		var remainder = time % waitTime;
		if (remainder < timingWindow || remainder > (waitTime - timingWindow))
		{
			return false;
		}
		return true;
	}
}