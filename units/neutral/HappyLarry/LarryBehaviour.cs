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
		Parent.Alliance = UnitAlliance.Hostile;
	}

	public override void _Ready()
	{
		AddChild(walkTimer);
		AddChild(chillTimer);
		walkTimer.Timeout += () =>
		{
			chillTimer.Start(2);
			SignalBus.SendMessage("Chilling");
		};

		chillTimer.Timeout += () =>
		{
			walkTimer.Start(2);
			Parent.Rotate(Vector3.Up, GD.Randf() * (float)Math.PI);
			SignalBus.SendMessage("Walking");
		};
		chillTimer.Start(2);

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
