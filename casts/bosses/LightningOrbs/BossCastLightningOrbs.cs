using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Project;
public partial class BossCastLightningOrbs : BaseCast
{
	readonly List<PowerUpLightningOrb> SpawnedOrbs = new();
	public BossCastLightningOrbs(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Lightning Orbs",
			TargetType = CastTargetType.None,
			InputType = CastInputType.AutoRelease,
			HoldTime = 32,
			RecastTime = 0,
			PrepareTime = 32,
			ChannelingTickTimings = BeatTime.One | BeatTime.Half,
		};
	}

	List<Vector3> GetSpawnPositions()
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

	protected override void OnCastStarted(CastTargetData _)
	{
		var positions = GetSpawnPositions();

		foreach (var pos in positions)
		{
			var token = Lib.Scene(Lib.Token.PowerUpLightningOrb).Instantiate<PowerUpLightningOrb>();
			token.Position = pos;
			GetTree().CurrentScene.AddChild(token);
			SpawnedOrbs.Add(token);
		}
	}
}