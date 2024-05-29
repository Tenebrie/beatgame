using System;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace Project;
public partial class BossLargeCornerAttack : BossGroundAttack
{
	public BossLargeCornerAttack(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			TargetType = CastTargetType.Point,
			InputType = CastInputType.AutoRelease,
			TargetAlliances = new() { UnitAlliance.Hostile },
			HoldTime = 4,
			RecastTime = 0,
		};
	}

	protected override void CastStarted(CastTargetData targetData)
	{
		var circle = this.CreateGroundCircularArea(targetData.Point);
		circle.Radius = AreaRadius;
		circle.GrowTime = this.Settings.HoldTime;
		circle.Alliance = UnitAlliance.Hostile;
	}

	protected override void CastOnPoint(Vector3 point)
	{
		var targets = BaseUnit.AllUnits
		.Where(unit =>
			unit.IsAlive
			&& unit.Alliance.HostileTo(Parent.Alliance)
			&& unit.Position.FlatDistanceTo(point) <= AreaRadius
			&& unit.Position.VerticalDistanceTo(point) <= 1);

		foreach (var target in targets)
		{
			target.Health.Damage(5);
		}
	}
}