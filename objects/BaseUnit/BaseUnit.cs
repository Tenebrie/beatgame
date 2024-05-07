using System.Collections.Generic;
using Godot;

namespace Project;

public abstract partial class BaseUnit : CharacterBody3D
{
	public ObjectResource hp;
	public ObjectTargeting targeting;

	public Alliance alliance = Alliance.Neutral;
	public List<ComposableScript> composables = new();

	public BaseUnit()
	{
		hp = new ObjectResource(this, ObjectResourceType.Health, max: 100);
		targeting = new ObjectTargeting(this);

		composables.Add(hp);
		composables.Add(targeting);
	}

	public override void _Ready()
	{
		foreach (var composable in composables)
		{
			composable._Ready();
		}
	}

	public override void _Process(double delta)
	{
		foreach (var composable in composables)
		{
			composable._Process(delta);
		}
	}

	public override void _Input(InputEvent @event)
	{
		foreach (var composable in composables)
		{
			composable._Input(@event);
		}
	}

	public override void _ExitTree()
	{
		foreach (var composable in composables)
		{
			composable._ExitTree();
		}
	}
}
