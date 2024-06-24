using Godot;
using Project;
using System;
using System.Diagnostics;

namespace Project;
public partial class FightStartTrigger : Area3D
{
	public override void _Ready()
	{
		BodyEntered += OnAreaEntered;
	}

	private void OnAreaEntered(Node3D body)
	{
		if (body is not PlayerController)
			return;

		TimelineManager.Singleton.StartFight();
	}
}
