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

	private ulong startTime;
	private ulong waitTime;
	private ulong lastTickedAt;
	private bool lockedState = true;
	private ulong beatCount = 0;

	private ulong timingWindow = 125; // ms before and after beat

	public void Start(float bpm)
	{
		waitTime = (ulong)Math.Floor(1 / bpm * 60 * 1000);
		startTime = Time.Singleton.GetTicksMsec();
		lastTickedAt = startTime;
	}

	public override void _Process(double delta)
	{
		var songTime = Time.Singleton.GetTicksMsec() - startTime;
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

	private ulong GetTickIndexAt(ulong time)
	{
		return time / waitTime;
	}

	private bool IsLockedAt(ulong time)
	{
		var remainder = time % waitTime;
		if (remainder < timingWindow || remainder > (waitTime - timingWindow))
		{
			return false;
		}
		return true;
	}
}