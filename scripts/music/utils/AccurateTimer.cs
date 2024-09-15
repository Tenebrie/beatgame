using System;
using Godot;
using Project;

namespace BeatGame.scripts.music;

public partial class AccurateTimer : Node
{
	[Signal] public delegate void TimeoutEventHandler();
	[Signal] public delegate void CatchUpTickEventHandler();

	public float Calibration = 0;

	private bool IsStarted;
	private float startTime;
	public float waitTime;
	private long tickOffset;
	public float LastTickedAt;
	public long TickIndex = -1;

	public void Start(float bpm)
	{
		IsStarted = true;
		waitTime = 1f / ((float)bpm / 60f);
		startTime = CastUtils.GetEngineTime();
		LastTickedAt = CastUtils.GetEngineTime();

		TickIndex = -1;
	}

	public async void Stop(float delay)
	{
		await ToSignal(GetTree().CreateTimer(delay), "timeout".ToStringName());
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
				EmitSignal(SignalName.CatchUpTick);
			}

			TickIndex = tickIndex;
			LastTickedAt = CastUtils.GetEngineTime();
			EmitSignal(SignalName.Timeout);
		}
	}

	long GetInternalTickIndexAtSongTime(float songTime)
	{
		if (waitTime <= 0)
			return 0;

		return (long)Math.Floor(songTime / waitTime);
	}

	public float GetSongTime()
	{
		var time = CastUtils.GetEngineTime();
		return time - startTime + Calibration;
	}
}