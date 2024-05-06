using Godot;
using System;
using System.Diagnostics;

namespace Project;

public partial class ProjectileImpact : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// GetNode<GpuParticles3D>("GPUParticles3D").OneShot = true;
		// GetNode<GpuParticles3D>("GPUParticles3D").Explosiveness = 1;
		GetNode<GpuParticles3D>("GPUParticles3D").Restart();
		Cleanup();
	}

	private async void Cleanup()
	{
		await ToSignal(GetTree().CreateTimer(2), "timeout");
		QueueFree();
	}
}
