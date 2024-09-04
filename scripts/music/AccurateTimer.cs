using System;
using Godot;

namespace Project;

public partial class AccurateTimer : Node
{
	[Signal] public delegate void TimeoutEventHandler(BeatTime time);
	[Signal] public delegate void CatchUpTickEventHandler(BeatTime time);

	public static float TimingWindow = 0.05f; // seconds
	public static float QueueingWindow = 30f; // seconds
	public long Calibration = 0;

	public BeatTime BeatTime;
	private bool IsStarted;
	private long startTime;
	public long waitTime;
	private long tickOffset;
	public float LastTickedAt;
	public long TickIndex = -1;

	public void Start(float bpm)
	{
		IsStarted = true;
		waitTime = (long)(1 / (bpm / 60) * 1000);
		startTime = (long)Time.Singleton.GetTicksMsec();
		LastTickedAt = CastUtils.GetEngineTime();

		TickIndex = -1;
	}

	public async void Stop(float delay)
	{
		await ToSignal(GetTree().CreateTimer(delay / 1000), "timeout".ToStringName());
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
			LastTickedAt = CastUtils.GetEngineTime();
			EmitSignal(SignalName.Timeout, BeatTime.ToVariant());
		}
	}

	long GetInternalTickIndexAtSongTime(long songTime)
	{
		if (waitTime == 0)
			return 0;

		return songTime / waitTime;
	}

	public long GetSongTime()
	{
		var time = (long)Time.Singleton.GetTicksMsec();
		return time - startTime + Calibration;
	}
}