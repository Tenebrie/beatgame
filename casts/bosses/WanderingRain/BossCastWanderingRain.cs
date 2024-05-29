using System;
using System.Collections.Generic;
using Godot;

namespace Project;
public partial class BossCastWanderingRain : BaseCast
{
	readonly List<WanderingRain> rains = new();

	public BossCastWanderingRain(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			InputType = CastInputType.Instant,
			TargetType = CastTargetType.None,
			HoldTime = 0,
			RecastTime = 0,
		};
	}

	List<(Vector3 position, float rotation)> GetSpawnPositions()
	{
		var list = new List<(Vector3, float)>();
		foreach (var facing in CastUtils.AllArenaFacings())
		{
			var angle = this.GetArenaFacingAngle(facing);
			list.Add((this.GetArenaEdgePosition(new Vector3(-1, 0, 0.1f - 0.05f), facing), this.GetArenaFacingAngle(ArenaFacing.East) + angle));
			list.Add((this.GetArenaEdgePosition(new Vector3(+1, 0, 0.2f - 0.05f), facing), this.GetArenaFacingAngle(ArenaFacing.West) + angle));
			list.Add((this.GetArenaEdgePosition(new Vector3(-1, 0, 0.3f - 0.05f), facing), this.GetArenaFacingAngle(ArenaFacing.East) + angle));
			list.Add((this.GetArenaEdgePosition(new Vector3(+1, 0, 0.4f - 0.05f), facing), this.GetArenaFacingAngle(ArenaFacing.West) + angle));
			list.Add((this.GetArenaEdgePosition(new Vector3(-1, 0, 0.5f - 0.05f), facing), this.GetArenaFacingAngle(ArenaFacing.East) + angle));
			list.Add((this.GetArenaEdgePosition(new Vector3(+1, 0, 0.6f - 0.05f), facing), this.GetArenaFacingAngle(ArenaFacing.West) + angle));
			list.Add((this.GetArenaEdgePosition(new Vector3(-1, 0, 0.7f - 0.05f), facing), this.GetArenaFacingAngle(ArenaFacing.East) + angle));
			list.Add((this.GetArenaEdgePosition(new Vector3(+1, 0, 0.8f - 0.05f), facing), this.GetArenaFacingAngle(ArenaFacing.West) + angle));
		}
		return list;
	}

	protected override void CastStarted(CastTargetData _)
	{

	}

	protected override void CastOnNone() { }

	private enum State
	{
		Spawning,
		Charging,
		Moving,
		Despawning,
		Finished,
	}
}