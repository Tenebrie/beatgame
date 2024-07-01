using Godot;
using System;

namespace Project;

public partial class AnimatedTrident : BasicEnemyController
{
	bool IsMoving = false;
	CircularTelegraph area;

	public override void _Ready()
	{
		Alliance = UnitAlliance.Hostile;

		base._Ready();

		Targetable.Untargetable = true;

		area = this.CreateCircularTelegraph(Position);
		area.Settings.GrowTime = 0;
		area.Settings.Radius = this.GetArenaSize() * 0.16f / 2;
		area.Settings.Periodic = true;
		area.Settings.Alliance = UnitAlliance.Hostile;
		area.Settings.TargetValidator = (unit) => unit.HostileTo(this);
		area.Settings.OnTargetEntered = OnImpactCallback;
	}

	public void OnImpactCallback(BaseUnit unit)
	{
		unit.Health.Damage(50f, this);
		var vector = Position - unit.Position;
		unit.ForcefulMovement.Push(Position.FlatDistanceTo(unit.Position), vector.Flatten(unit.Position.Y), 0.5f);
		unit.Buffs.Add(new BuffTridentTargetSlow());

		var circle = this.CreateCircularTelegraph(this.GetGroundedPosition());
		circle.Settings.Radius = 3;
		circle.Settings.GrowTime = 2;
		circle.Settings.TargetValidator = (unit) => unit.HostileTo(this);
		circle.Settings.OnFinishedPerTargetCallback = (unit) =>
		{
			unit.Health.Damage(75f, this);
			unit.ForcefulMovement.AddInertia(5, Vector3.Up);
		};

		QueueFree();

		if (IsInstanceValid(area))
			area.CleanUp();
	}

	public void Activate(float distance, float time)
	{
		IsMoving = true;
		Velocity = ForwardVector * (distance / time);
	}

	public void SetActive(bool active)
	{
		IsMoving = active;
		if (active == false && IsInstanceValid(area))
			area.CleanUp();
	}

	public override void _Process(double delta)
	{
		if (IsInstanceValid(area))
			area.Position = Position;

		if (!IsMoving)
			return;

		Position += Velocity * (float)delta;
	}
}
