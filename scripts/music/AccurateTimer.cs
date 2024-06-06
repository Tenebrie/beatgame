using System;
using System.Diagnostics;
using Godot;

namespace Project;

public partial class AccurateTimer : Node
{
	[Signal]
	public delegate void TimeoutEventHandler(BeatTime time);
	[Signal]
	public delegate void CatchupTickEventHandler(BeatTime time);
	[Signal]
	public delegate void BeatWindowUnlockEventHandler(BeatTime time);
	[Signal]
	public delegate void BeatWindowLockEventHandler(BeatTime time);

	public const long TimingWindow = 100; // ms before and after beat
	public long Calibration = 0;
	public AccurateTimer PrecedingTimer;

	public BeatTime BeatTime;
	private bool IsStarted;
	private long startTime;
	private long waitTime;
	public long LastTickedAt;
	private bool timingWindowLocked = true;
	private long publishedTickIndex = -1;
	public long TickIndex = -1;
	public float SpeedFactor;

	public void Start(float bpm, float speedFactor)
	{
		IsStarted = true;
		waitTime = (long)Math.Floor(1 / (bpm * speedFactor) * 60 * 1000);
		startTime = (long)Time.Singleton.GetTicksMsec();
		LastTickedAt = startTime;
		SpeedFactor = speedFactor;
		TickIndex = -1;
		publishedTickIndex = -1;
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
		if (timingWindowLocked && !locked && (PrecedingTimer == null || PrecedingTimer.IsUnlockedNow()))
		{
			timingWindowLocked = false;
			EmitSignal(SignalName.BeatWindowUnlock, BeatTime.ToVariant());
		}
		else if (!timingWindowLocked && locked)
		{
			timingWindowLocked = true;
			EmitSignal(SignalName.BeatWindowLock, BeatTime.ToVariant());
		}

		var tickIndex = GetTickIndexAtSongTime(songTime);
		if (tickIndex > publishedTickIndex)
		{
			if (PrecedingTimer == null || PrecedingTimer.IsUnlockedNow())
			{
				publishedTickIndex = tickIndex;
				LastTickedAt = time;
				TickIndex += 1;
				EmitSignal(SignalName.Timeout, BeatTime.ToVariant());

				// for (var i = 0; i < tickIndex - publishedTickIndex - 1; i++)
				// {
				// 	TickIndex += 1;
				// 	EmitSignal(SignalName.CatchupTick, BeatTime.ToVariant());
				// }
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
		return timingWindowLocked && (PrecedingTimer == null || PrecedingTimer.IsUnlockedNow());
	}

	public long GetSongTime()
	{
		var time = (long)Time.Singleton.GetTicksMsec();
		return time - startTime + Calibration;
	}
}