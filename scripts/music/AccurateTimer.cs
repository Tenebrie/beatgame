using System;
using Godot;

namespace Project;

public partial class AccurateTimer : Node
{
	[Signal]
	public delegate void TimeoutEventHandler(BeatTime time);
	[Signal]
	public delegate void CatchUpTickEventHandler(BeatTime time);
	[Signal]
	public delegate void BeatWindowUnlockEventHandler(BeatTime time);
	[Signal]
	public delegate void BeatWindowLockEventHandler(BeatTime time);

	public static float TimingWindow
	{
		get => Preferences.Singleton.ChillMode ? 0.1f : 0.2f; // seconds before and after beat
	}
	public long Calibration = 0;

	public BeatTime BeatTime;
	private bool IsStarted;
	private long startTime;
	public long waitTime;
	private long tickOffset;
	public long LastTickedAt;
	private bool timingWindowLocked = true;
	public long TickIndex = -1;
	private float SpeedModifier;

	public void Start(float bpm, float speedModifier, int halfOffsets)
	{
		float ticksPerMinute = bpm * speedModifier;
		IsStarted = true;
		waitTime = (long)(1 / (ticksPerMinute / 60) * 1000);
		tickOffset = 0;
		tickOffset = (long)(waitTime / 2.0f * halfOffsets);
		startTime = (long)Time.Singleton.GetTicksMsec();
		LastTickedAt = startTime;
		SpeedModifier = speedModifier;

		TickIndex = -1;
		if (halfOffsets > 0)
			TickIndex = 0;
	}

	public async void Stop(float delay)
	{
		await ToSignal(GetTree().CreateTimer(delay / 1000), "timeout");
		IsStarted = false;
	}

	public override void _Process(double delta)
	{
		if (!IsStarted)
			return;

		var songTime = GetSongTime();

		if (songTime < 0)
			return;

		var tickIndex = GetInternalTickIndexAtSongTime(songTime);
		if (tickIndex > TickIndex)
		{
			for (var i = 0; i < tickIndex - TickIndex - 1; i++)
			{
				EmitSignal(SignalName.CatchUpTick, BeatTime.ToVariant());
			}

			TickIndex = tickIndex;
			LastTickedAt = (long)Time.Singleton.GetTicksMsec();
			EmitSignal(SignalName.Timeout, BeatTime.ToVariant());
		}
	}

	long GetInternalTickIndexAtSongTime(long songTime)
	{
		if (waitTime == 0)
			return 0;

		return songTime / waitTime;
	}

	public double GetNearestBeatIndex()
	{
		if (waitTime == 0)
			return 0;

		var deltaTime = GetTimeToNearestTick();
		var offset = tickOffset > 0 ? 0.5f : 0;
		if (deltaTime <= 0)
		{
			return (TickIndex - offset) / SpeedModifier;
		}
		else
		{
			return (TickIndex + 1 - offset) / SpeedModifier;
		}
	}

	public long GetTimeToNearestTick()
	{
		var time = (long)Time.GetTicksMsec();
		var expectedTickAt = LastTickedAt + waitTime;
		if (expectedTickAt - time > time - LastTickedAt)
			return LastTickedAt - time;
		return expectedTickAt - time;
	}

	public long GetSongTime()
	{
		var time = (long)Time.Singleton.GetTicksMsec();
		return time + tickOffset - startTime + Calibration;
	}
}