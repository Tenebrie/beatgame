using System;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace Project;
public partial class BossGroundAttack : BaseCast
{
	private float AreaRadius = 0.5f;
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

	protected override void CastOnPoint(Vector3 point)
	{
		var circle = this.CreateGroundCircularArea(point);
		circle.Radius = AreaRadius;
		circle.GrowTime = 1;
		circle.Alliance = UnitAlliance.Hostile;
		circle.OnFinishedCallback = () => OnFinishedCasting(point);
	}

	private void OnFinishedCasting(Vector3 point)
	{
		var targets = BaseUnit.AllUnits
		.Where(unit =>
			unit.Alliance.HostileTo(Parent.Alliance)
			&& unit.Position.FlatDistanceTo(point) <= AreaRadius
			&& unit.Position.VerticalDistanceTo(point) <= 1);

		foreach (var target in targets)
		{
			target.Health.Damage(5);
		}
	}
}