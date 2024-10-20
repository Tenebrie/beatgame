using Godot;
using System.Collections.Generic;

namespace Project;

public partial class BeatIndicator : Control
{
	public MeshInstance2D MainBeat;
	public List<List<BeatBar>> BarGroups = new();
	public override void _Ready()
	{
		Music.Singleton.BeatTick += OnBeat;
		Music.Singleton.VisualBeatTimer.Timeout += OnVisualBeat;
		LoadingManager.Singleton.SceneChanged += OnSceneChanged;
	}

	public void OnBeat(BeatTime time)
	{
		if ((time & (BeatTime.Whole | BeatTime.Half | BeatTime.Quarter)) == 0)
			return;

		if (BarGroups.Count == 0)
			return;

		foreach (var bar in BarGroups[0])
		{
			bar.Position = new Vector2(0, 0);
			bar.Cleanup();
		}
		BarGroups.RemoveAt(0);
	}

	public void OnVisualBeat()
	{
		var time = CastUtils.GetEngineTime();
		var endsAt = time + Music.Singleton.settings.SongDelay;

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

	void OnSceneChanged(PlayableScene _)
	{
		foreach (var barGroup in BarGroups)
			foreach (var bar in barGroup)
				bar.QueueFree();
		BarGroups.Clear();
	}
}
