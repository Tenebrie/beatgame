using Godot;
using System;

namespace Project;

public partial class AnimatedTrident : BasicEnemyController
{
	bool IsMoving = false;
	GroundAreaCircle area;

	public override void _Ready()
	{
		area = this.CreateGroundCircularArea(this.Position);
		area.GrowTime = 0;
		area.Radius = this.GetArenaSize() * 0.16f / 2;
		area.Periodic = true;
		area.Alliance = UnitAlliance.Hostile;
		area.OnTargetEntered = OnImpactCallback;
	}

	public void OnImpactCallback(BaseUnit unit)
	{
		unit.Health.Damage(50f);
		unit.ForcefulMovement.Push(2, unit.Position - Position, 0.25f);
	}

	public void Activate(float distance, float time)
	{
		IsMoving = true;
		Velocity = ForwardVector * (distance / time);
	}

	public void SetActive(bool active)
	{
		IsMoving = active;
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
