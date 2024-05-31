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
			FriendlyName = "Consuming Winds",
			TargetType = CastTargetType.None,
			InputType = CastInputType.AutoRelease,
			HoldTime = 8,
			RecastTime = 0,
			PrepareTime = 2,
			ChannelingTickTimings = BeatTime.One | BeatTime.Half,
		};
	}

	protected override void OnPrepCompleted(CastTargetData _)
	{
		List<ObjectForcefulMovement.ContinuousMovement> movements = new();
		var targets = BaseUnit.AllUnits.Where(unit => unit.HostileTo(Parent) && unit.Position.DistanceTo(Parent.Position) > 2);
		foreach (var target in targets)
		{
			Vector3 getVector() => (Parent.Position - target.Position).Flatten(target.Position.Y);
			var movement = target.ForcefulMovement.PushContinuously(() => 1.5f, getVector);
			movements.Add(movement);
			var jumpingMovement = target.ForcefulMovement.PushContinuously(() => 2.5f, getVector, () => target.Grounded);
			movements.Add(jumpingMovement);
		}

		var circle = this.CreateGroundCircularArea(Parent.GetGroundedPosition());
		circle.Radius = 12;
		circle.GrowTime = Settings.HoldTime;
		circle.Alliance = UnitAlliance.Hostile;
		circle.OnFinishedPerTargetCallback = (BaseUnit target) =>
		{
			target.Health.Damage(80);
			target.ForcefulMovement.Push(12, target.Position - Parent.Position, 2);
		};
		circle.OnFinishedCallback = () =>
		{
			foreach (var movement in movements)
				movement.Stop();
		};
	}
}