using Godot;
using Project;
using System;
using System.Diagnostics;

public partial class SceneTransition : Area3D
{
	[Export]
	private PackedScene TransitionTo;

	public override void _Ready()
	{
		BodyEntered += OnAreaEntered;
	}

	private void OnAreaEntered(Node3D body)
	{
		if (body is not PlayerController)
			return;

		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.SceneTransitionStarted, TransitionTo);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
