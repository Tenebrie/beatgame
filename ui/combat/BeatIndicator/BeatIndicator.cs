using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Project;

public partial class BeatIndicator : Control
{
	public MeshInstance2D MainBeat;
	public List<List<BeatBar>> BarGroups = new();
	public override void _Ready()
	{
		Music.Singleton.BeatTimer.Timeout += OnBeat;
		Music.Singleton.VisualBeatTimer.Timeout += OnVisualBeat;
	}

	public void OnBeat()
	{
		foreach (var bar in BarGroups[0])
		{
			bar.Position = new Vector2(0, 0);
			bar.Cleanup();
		}
		BarGroups.RemoveAt(0);
	}

	public void OnVisualBeat()
	{
		var time = (long)Time.Singleton.GetTicksMsec();
		var endsAt = time + Music.Singleton.SongDelay;

		var barGroup = new List<BeatBar>();
		var rightBar = new BeatBar(true)
		{
			StartsAt = time,
			EndsAt = endsAt,
		};
		AddChild(rightBar);
		barGroup.Add(rightBar);

		var leftBar = new BeatBar(false)
		{
			StartsAt = time,
			EndsAt = endsAt,
		};
		AddChild(leftBar);
		barGroup.Add(leftBar);
		BarGroups.Add(barGroup);
	}
}
