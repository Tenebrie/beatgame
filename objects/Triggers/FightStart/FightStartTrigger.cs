using Godot;
using Project;
using System;
using System.Diagnostics;

namespace Project;
public partial class FightStartTrigger : Area3D
{
	public override void _Ready()
	{
		BodyExited += OnAreaExited;
	}

	private void OnAreaExited(Node3D body)
	{
		if (body is not PlayerController)
			return;

		Music.Singleton.Start();
	}
}
