using Godot;
using Project;
using System;
using System.Diagnostics;

public partial class JumpingDummyEnemy : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var timer = GetNode<Timer>("Timer");
		timer.Timeout += OnTimerTick;
	}

	public void OnTimerTick()
	{
		var player = PlayerController.All[0];
		var distance = player.Position.DistanceTo(Position);
		if (distance < 5)
		{
			player.hp.Damage(5);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
}
