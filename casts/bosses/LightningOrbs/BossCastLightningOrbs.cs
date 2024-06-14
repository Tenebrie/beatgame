using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Project;
public partial class BossCastLightningOrbs : BaseCast
{
	readonly List<LightningOrbsPylon> SpawnedPylons = new();

	public BossCastLightningOrbs(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Lightning Orbs",
			TargetType = CastTargetType.None,
			InputType = CastInputType.AutoRelease,
			HoldTime = 31,
			RecastTime = 0,
			PrepareTime = 32,
		};
	}

	List<Vector3> GetOrbsSpawnPositions()
	{
		var list = new List<Vector3>();
		foreach (var facing in CastUtils.AllArenaFacings())
		{
			list.Add(this.GetArenaEdgePosition(new Vector3(+0.10f, 0, 0.10f), facing));
			list.Add(this.GetArenaEdgePosition(new Vector3(-0.10f, 0, 0.10f), facing));
			list.Add(this.GetArenaEdgePosition(new Vector3(0f, 0, 0.20f), facing));
			list.Add(this.GetArenaEdgePosition(new Vector3(+0.10f, 0, 0.30f), facing));
			list.Add(this.GetArenaEdgePosition(new Vector3(-0.10f, 0, 0.30f), facing));
		}
		return list;
	}

	static List<Vector3> GetPylonsSpawnPositions()
	{
		var list = new List<Vector3>()
		{
			new(+4, 0, +4),
			new(+4, 0, -4),
			new(-4, 0, +4),
			new(-4, 0, -4),
		};
		return list;
	}

	protected override void OnCastStarted(CastTargetData _)
	{
		SpawnedPylons.Clear();

		var orbPositions = GetOrbsSpawnPositions();
		foreach (var pos in orbPositions)
		{
			var token = Lib.LoadScene(Lib.Token.PowerUpLightningOrb).Instantiate<PowerUpLightningOrb>();
			token.Position = pos;
			GetTree().CurrentScene.AddChild(token);
		}

		var pylonPositions = GetPylonsSpawnPositions();
		foreach (var pos in pylonPositions)
		{
			var token = Lib.LoadScene(Lib.Token.LightningOrbsPylon).Instantiate<LightningOrbsPylon>();
			token.Position = pos;
			GetTree().CurrentScene.AddChild(token);
			SpawnedPylons.Add(token);
		}
	}

	protected override void OnPrepCompleted(CastTargetData _)
	{
		var pylons = SpawnedPylons.Where(pylon => IsInstanceValid(pylon));
		foreach (var pylon in pylons)
		{
			pylon.Activate();
		}
	}

	protected override void OnCastCompleted(CastTargetData _)
	{
		var pylons = SpawnedPylons.Where(pylon => IsInstanceValid(pylon));
		foreach (var pylon in pylons)
		{
			pylon.PerformFinalCast();
		}
	}
}