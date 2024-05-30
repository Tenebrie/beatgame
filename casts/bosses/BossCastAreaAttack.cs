using System;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace Project;
public partial class BossCastAreaAttack : BaseCast
{
	public float AreaRadius = 8;

	public BossCastAreaAttack(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			TargetType = CastTargetType.Point,
			InputType = CastInputType.AutoRelease,
			TargetAlliances = new() { UnitAlliance.Hostile },
			HoldTime = 8,
			RecastTime = 0,
		};
	}

	protected override void OnCastStarted(CastTargetData targetData)
	{
		foreach (var point in targetData.MultitargetPoints)
		{
			var circle = this.CreateGroundCircularArea(point);
			circle.Radius = AreaRadius;
			circle.GrowTime = Settings.HoldTime;
			circle.Alliance = UnitAlliance.Hostile;
			circle.OnFinishedPerTargetCallback = (target) =>
			{
				target.Health.Damage(40);
			};
		}
	}
}