using Godot;
using Project;
using System;

namespace Project;
public partial class PushAwayZone : Area3D
{
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	void OnBodyEntered(Node3D body)
	{
		if (body is not BaseUnit unit)
			return;

		unit.Velocity = new Vector3(unit.Velocity.X, 0, unit.Velocity.Z);
		unit.ForcefulMovement.AddInertia(18, Vector3.Up);
		unit.Health.Damage(25f, null, null);
	}
}
