using Godot;
using System.Collections.Generic;

namespace Project;

public partial class BeatIndicator : Control
{
	public MeshInstance2D MainBeat;
	public List<List<BeatBar>> BarGroups = new();
	public override void _Ready()
	{
		Music.Singleton.BeatTimer.Timeout += OnBeat;
		Music.Singleton.HalfBeatTimer.Timeout += OnHalfBeat;
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

	public void OnHalfBeat()
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

		var isHalfBeat = Music.Singleton.VisualBeatTimer.TickIndex % 2 == 1;
		var barGroup = new List<BeatBar>();
		var rightBar = new BeatBar(mirrored: true, halfBeat: isHalfBeat)
		{
			StartsAt = time,
			EndsAt = endsAt,
		};
		AddChild(rightBar);
		barGroup.Add(rightBar);

		var leftBar = new BeatBar(false, halfBeat: isHalfBeat)
		{
			StartsAt = time,
			EndsAt = endsAt,
		};
		AddChild(leftBar);
		barGroup.Add(leftBar);
		BarGroups.Add(barGroup);
	}
}
