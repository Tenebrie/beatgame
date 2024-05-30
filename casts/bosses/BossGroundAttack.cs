using System;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace Project;
public partial class BossGroundAttack : BaseCast
{
	public float AreaRadius = 0.5f;

	public BossGroundAttack(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			TargetType = CastTargetType.Point,
			InputType = CastInputType.AutoRelease,
			TargetAlliances = new() { UnitAlliance.Hostile },
			HoldTime = 1,
			RecastTime = 0,
		};
	}

	protected override void OnCastStarted(CastTargetData targetData)
	{
		var circle = this.CreateGroundCircularArea(targetData.Point);
		circle.Radius = AreaRadius;
		circle.GrowTime = this.Settings.HoldTime;
		circle.Alliance = UnitAlliance.Hostile;
	}

	protected override void OnCastCompleted(CastTargetData target)
	{
		var targets = BaseUnit.AllUnits
		.Where(unit =>
			unit.IsAlive
			&& unit.Alliance.HostileTo(Parent.Alliance)
			&& unit.Position.FlatDistanceTo(target.Point) <= AreaRadius
			&& unit.Position.VerticalDistanceTo(target.Point) <= AreaRadius);

		foreach (var t in targets)
		{
			t.Health.Damage(5);
		}
	}
}