using System.Collections.Generic;
using Godot;

namespace Project;

public partial class BossCastGeysers : BaseCast
{
	readonly List<BaseUnit> affectedUnits = new();
	public VariationType Variation = 0;

	public BossCastGeysers(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Geysers",
			InputType = CastInputType.AutoRelease,
			HoldTime = 4,
			RecastTime = 0,
			PrepareTime = 16,
		};
	}

	protected override void OnCastStarted(CastTargetData _)
	{
		var arenaSize = this.GetArenaSize();
		var offset = 0.65f;
		List<Vector3> spawns = new();
		if ((Variation & VariationType.UnderBoss) > 0)
		{
			spawns.Add(new Vector3(0, 0, 0));
		}

		if ((Variation & VariationType.FourSides) > 0)
		{
			spawns.Add(new Vector3(+arenaSize * offset, 0, 0));
			spawns.Add(new Vector3(-arenaSize * offset, 0, 0));
			spawns.Add(new Vector3(0, 0, +arenaSize * offset));
			spawns.Add(new Vector3(0, 0, -arenaSize * offset));
		}

		if ((Variation & VariationType.FourCorners) > 0)
		{
			spawns.Add(new Vector3(+arenaSize * offset, 0, +arenaSize * offset));
			spawns.Add(new Vector3(+arenaSize * offset, 0, -arenaSize * offset));
			spawns.Add(new Vector3(-arenaSize * offset, 0, +arenaSize * offset));
			spawns.Add(new Vector3(-arenaSize * offset, 0, -arenaSize * offset));
		}

		foreach (var spawn in spawns)
		{
			var circle = this.CreateGroundCircularArea(spawn);
			circle.Radius = 2;
			circle.GrowTime = Settings.PrepareTime;
			circle.Alliance = UnitAlliance.Neutral;
			circle.OnFinishedPerTargetCallback = (unit) =>
			{
				unit.Buffs.Add(new BuffGeyserLevitation()
				{
					Duration = Settings.HoldTime + 4,
				});
				unit.ForcefulMovement.Push(10, Vector3.Up, 3f);
				affectedUnits.Add(unit);
			};
		}
	}

	protected override void OnCastCompleted(CastTargetData _)
	{
		foreach (var unit in affectedUnits)
		{
			unit.ForcefulMovement.Push(10, Vector3.Down, 2, inverted: true);
		}
	}

	public enum VariationType : int
	{
		UnderBoss = 1,
		FourSides = 2,
		FourCorners = 4,
		AllPositions = 7,
	}
}