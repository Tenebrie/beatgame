using Godot;
using Project;
using System;

namespace Project;

public partial class LarryBehaviour : BaseBehaviour
{
	Timer walkTimer = new() { OneShot = true };
	Timer chillTimer = new() { OneShot = true };

	public override void _EnterTree()
	{
		Parent.FriendlyName = "Larry";
		Parent.Targetable.SelectionRadius = 0.3f;
	}

	public override void _Ready()
	{
		walkTimer.Timeout += () =>
		{
			chillTimer.Start(2 + GD.Randf());
		};
		AddChild(walkTimer);

		chillTimer.Timeout += () =>
		{
			walkTimer.Start(2 + GD.Randf());
			Parent.Rotate(Vector3.Up, GD.Randf() * (float)Math.PI);
		};
		AddChild(chillTimer);
		chillTimer.Start(2 + GD.Randf());

		GetComponent<AnimationController>().RegisterStateTransitions(
			("Bite", (_) => chillTimer.TimeLeft >= 1.2),
			("Walk", (_) => !walkTimer.IsStopped()),
			("Idle", (_) => true)
		);
	}

	public override void _Process(double delta)
	{
		if (!walkTimer.IsStopped())
			Parent.Velocity = Parent.ForwardVector * 0.5f;
		else
			Parent.Velocity = Vector3.Zero;

		Parent.MoveAndSlide();
	}
}
