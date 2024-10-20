using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace Project;
public partial class BossCastQuickRavagingWinds : BaseCast
{
	public float Rotation = 0;
	public float AreaRadius = 3;

	public BossCastQuickRavagingWinds(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Quickened Ravaging Winds",
			TargetType = CastTargetType.None,
			InputType = CastInputType.AutoRelease,
			HoldTime = 10,
			RecastTime = 0,
			PrepareTime = 4,
			TickDuration = 2,
		};
	}

	protected override void OnPrepCompleted(CastTargetData targetData)
	{
		OnCastTicked(targetData);
	}

	List<(Vector3 pos, float size)> GetSpawnPositions()
	{
		var list = new List<(Vector3 pos, float size)>();
		foreach (var facing in CastUtils.AllArenaFacings())
		{
			list.Add((this.GetArenaEdgePosition(new Vector3(0, 0, -0.10f), facing), 1.2f));
			list.Add((this.GetArenaEdgePosition(new Vector3(0, 0, +0.00f), facing), 1.1f));
			list.Add((this.GetArenaEdgePosition(new Vector3(0, 0, +0.10f), facing), 1.0f));
			list.Add((this.GetArenaEdgePosition(new Vector3(0, 0, +0.20f), facing), 0.9f));
			list.Add((this.GetArenaEdgePosition(new Vector3(0, 0, +0.30f), facing), 0.8f));
			list.Add((this.GetArenaEdgePosition(new Vector3(0, 0, +0.40f), facing), 0.7f));
			list.Add((this.GetArenaEdgePosition(new Vector3(0, 0, +0.50f), facing), 0.6f));
			list.Add((this.GetArenaEdgePosition(new Vector3(0, 0, +0.60f), facing), 0.5f));
			list.Add((this.GetArenaEdgePosition(new Vector3(0, 0, +0.70f), facing), 0.4f));
			list.Add((this.GetArenaEdgePosition(new Vector3(0, 0, +0.80f), facing), 0.3f));
			list.Add((this.GetArenaEdgePosition(new Vector3(0, 0, +0.90f), facing), 0.3f));
		}
		return list;
	}

	protected override void OnCastTicked(CastTargetData targetData)
	{
		var spawns = GetSpawnPositions();
		List<CircularTelegraph> circleGroup = new();
		foreach (var (spawn, size) in spawns)
		{
			var rotatedSpawn = spawn.Rotated(Vector3.Up, Rotation);
			var circle = this.CreateCircularTelegraph(rotatedSpawn);
			circle.Settings.Radius = size * AreaRadius;
			circle.Settings.GrowTime = 2;
			circle.Settings.Alliance = UnitAlliance.Hostile;
			circleGroup.Add(circle);
		}
		circleGroup[0].Settings.OnFinishedCallback = () =>
		{
			var targets = circleGroup.SelectMany(circle => circle.GetTargets()).Distinct().Where(unit => unit.HostileTo(Parent));
			foreach (var target in targets)
			{
				target.Health.Damage(30, this);
				target.ForcefulMovement.Push(2, (target.Position - Parent.Position).Flatten(target.Position.Y), 0.5f);
			}
		};
		Rotation += (float)Math.PI * 2 / Settings.HoldTime / 4;
	}
}