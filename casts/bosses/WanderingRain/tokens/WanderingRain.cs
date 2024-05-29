using Godot;
using System;

namespace Project;

public partial class WanderingRain : BasicEnemyController
{
	bool IsMoving = false;
	GroundAreaCircle area;

	public override void _Ready()
	{

	}

	public void Activate()
	{
		area = this.CreateGroundCircularArea(this.Position);
		area.GrowTime = 1;
		area.Radius = 1;
		area.OnHostileImpactCallback = OnImpactCallback;
	}

	public static void OnImpactCallback(BaseUnit unit)
	{
		unit.Health.Damage(10f);
	}

	public void Activate(float speed)
	{
		IsMoving = true;
		Velocity = ForwardVector * speed;
	}

	public void SetActive(bool active)
	{
		IsMoving = active;

	}

	public override void _Process(double delta)
	{
		if (!IsMoving)
			return;

		Position += Velocity * (float)delta;
	}
}
