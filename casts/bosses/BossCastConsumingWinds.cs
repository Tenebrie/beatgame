using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Project;
public partial class BossCastConsumingWinds : BaseCast
{
	public float Rotation = 0;
	public float AreaRadius = 3;

	public BossCastConsumingWinds(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			TargetType = CastTargetType.None,
			InputType = CastInputType.AutoRelease,
			HoldTime = 8,
			RecastTime = 0,
			ChannelingTickTimings = BeatTime.One | BeatTime.Half,
		};
	}

	protected override void OnCastStarted(CastTargetData _)
	{
		List<ObjectForcefulMovement.ContinuousMovement> movements = new();
		var targets = BaseUnit.AllUnits.Where(unit => unit.HostileTo(Parent) && unit.Position.DistanceTo(Parent.Position) > 2);
		foreach (var target in targets)
		{
			var movement = target.ForcefulMovement.PushContinuously(2, Parent.Position - target.Position);
			movements.Add(movement);
		}

		var circle = this.CreateGroundCircularArea(Parent.GetGroundedPosition());
		circle.Radius = 8;
		circle.GrowTime = Settings.HoldTime;
		circle.Alliance = UnitAlliance.Hostile;
		circle.OnFinishedPerTargetCallback = (BaseUnit target) =>
		{
			target.Health.Damage(80);
			target.ForcefulMovement.Push(12, target.Position - Parent.Position, 2);
			foreach (var movement in movements)
				movement.Stop();
		};
	}
}