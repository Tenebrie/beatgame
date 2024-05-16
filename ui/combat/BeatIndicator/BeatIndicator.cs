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
		// MainBeat.Scale = new Vector2(1, 1);
		var barGroup = new List<BeatBar>();
		var rightBar = new BeatBar(true);
		AddChild(rightBar);
		rightBar.Position = new Vector2(500, 0);
		barGroup.Add(rightBar);
		var leftBar = new BeatBar(false);
		AddChild(leftBar);
		leftBar.Position = new Vector2(-500, 0);
		barGroup.Add(leftBar);
		BarGroups.Add(barGroup);
	}

	public override void _Process(double delta)
	{
		float speed = 1000 * (float)500 / Music.Singleton.SongDelay * (float)delta;
		foreach (var barGroup in BarGroups)
		{
			foreach (var bar in barGroup)
			{
				float dir = -Math.Sign(bar.Position.X);
				bar.Position = new Vector2(bar.Position.X + speed * dir, bar.Position.Y);
			}
		}
	}
}
