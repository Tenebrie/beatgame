using Godot;
using System;

namespace Project;

public partial class WanderingRain : BasicEnemyController
{
	bool IsMoving = false;
	CircularTelegraph area;

	public override void _Ready()
	{

	}

	public void Activate()
	{
		area = this.CreateCircularTelegraph(this.Position);
		area.Settings.GrowTime = 1;
		area.Settings.Radius = 1;
		area.Settings.OnTargetEntered = OnImpactCallback;
	}

	public void OnImpactCallback(BaseUnit unit)
	{
		unit.Health.Damage(10f, this);
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
