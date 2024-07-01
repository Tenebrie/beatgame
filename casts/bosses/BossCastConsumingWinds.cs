using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Project;
public partial class BossCastConsumingWinds : BaseCast
{
	public float Damage = 80;
	public float PushDistance = 12;
	public float PullStrength = 0.5f;
	public float ExtraPullStrength = 1.5f;
	public float AreaRadius = 12;

	public BossCastConsumingWinds(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Consuming Winds",
			Description = $"Pull enemies closer to finish them off with a powerful blast.",
			IconPath = "res://assets/icons/SpellBook06_97.png",
			TargetType = CastTargetType.None,
			InputType = CastInputType.AutoRelease,
			HoldTime = 16,
			RecastTime = 0,
			PrepareTime = 8,
		};
	}

	protected override void OnPrepCompleted(CastTargetData _)
	{
		List<ObjectForcefulMovement.ContinuousMovement> movements = new();
		var targets = BaseUnit.AllUnits.Where(unit => unit.HostileTo(Parent) && unit.Position.DistanceTo(Parent.Position) > 2);
		foreach (var target in targets)
		{
			Vector3 getVector() => (Parent.Position - target.Position).Flatten(target.Position.Y);
			var movement = target.ForcefulMovement.PushContinuously(() => PullStrength, getVector);
			movements.Add(movement);
			var onlyGroundedMovement = target.ForcefulMovement.PushContinuously(() => ExtraPullStrength, getVector, () => target.Grounded);
			movements.Add(onlyGroundedMovement);
		}

		var circle = this.CreateCircularTelegraph(Parent.GetGroundedPosition());
		circle.Settings.Radius = AreaRadius;
		circle.Settings.GrowTime = Settings.HoldTime;
		circle.Settings.Alliance = Parent.Alliance;
		circle.Settings.TargetValidator = (target) => target.HostileTo(Parent);
		circle.Settings.OnFinishedPerTargetCallback = (BaseUnit target) =>
		{
			target.Health.Damage(Damage, this);
			target.ForcefulMovement.Push(PushDistance, (target.Position - Parent.Position).Flatten(target.Position.Y), 2);
		};
		circle.Settings.OnFinishedCallback = () =>
		{
			foreach (var movement in movements)
				movement.Stop();
		};
	}
}