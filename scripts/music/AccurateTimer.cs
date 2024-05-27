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

	private bool IsStarted;
	private long startTime;
	private long waitTime;
	public long LastTickedAt;
	private bool timingWindowLocked = true;
	private long internalTickIndex = -1;
	public long TickIndex = -1;

	public void Start(float bpm)
	{
		IsStarted = true;
		waitTime = (long)Math.Floor(1 / bpm * 60 * 1000);
		startTime = (long)Time.Singleton.GetTicksMsec();
		LastTickedAt = startTime;
		TickIndex = -1;
		internalTickIndex = -1;
	}

	public async void Stop(float delay)
	{
		await ToSignal(GetTree().CreateTimer(delay / 1000), "timeout");
		if (!timingWindowLocked)
		{
			timingWindowLocked = true;
			EmitSignal(SignalName.BeatWindowLock);
		}
		IsStarted = false;
	}

	public override void _Process(double delta)
	{
		if (!IsStarted)
			return;

		var time = (long)Time.Singleton.GetTicksMsec();
		var songTime = time - startTime + Calibration;

		if (songTime < 0)
			return;

		var locked = IsLockedAt(songTime);
		if (timingWindowLocked && !locked)
		{
			timingWindowLocked = false;
			EmitSignal(SignalName.BeatWindowUnlock);
		}
		else if (!timingWindowLocked && locked)
		{
			timingWindowLocked = true;
			EmitSignal(SignalName.BeatWindowLock);
		}

		var tickIndex = GetTickIndexAtSongTime(songTime);
		if (tickIndex > internalTickIndex)
		{
			internalTickIndex = tickIndex;
			if (PrecedingTimer == null || PrecedingTimer.IsUnlockedNow())
			{
				TickIndex += 1;
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

	public double GetNearestTickIndexAtEngineTime()
	{
		if (waitTime == 0)
			return 0;

		var time = (long)Time.Singleton.GetTicksMsec();
		var songTime = time - startTime + Calibration;
		return Math.Round((double)songTime / waitTime);
	}

	private bool IsLockedAt(long time)
	{
		var remainder = Math.Abs(time) % waitTime;
		if (remainder < TimingWindow || remainder > (waitTime - TimingWindow))
		{
			return false;
		}
		return true;
	}

	public bool IsUnlockedNow()
	{
		return timingWindowLocked;
	}
}