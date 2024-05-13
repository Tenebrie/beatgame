using System.Collections.Generic;
using Godot;

namespace Project;

public abstract partial class ComposableCharacterBody3D : CharacterBody3D
{
	public List<ComposableScript> Composables = new();

	public override void _Ready()
	{
		foreach (var composable in Composables)
		{
			composable._Ready();
		}
	}

	public override void _Process(double delta)
	{
		foreach (var composable in Composables)
		{
			composable._Process(delta);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
	}

	public override void _Input(InputEvent @event)
	{
		foreach (var composable in Composables)
		{
			composable._Input(@event);
		}
	}

	public override void _ExitTree()
	{
		foreach (var composable in Composables)
		{
			composable._ExitTree();
		}
	}
}
