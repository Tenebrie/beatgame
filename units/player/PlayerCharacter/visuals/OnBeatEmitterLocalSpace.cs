using System;
using System.Diagnostics;
using Godot;

namespace Project;

public partial class OnBeatEmitterLocalSpace : GpuParticles3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Music.Singleton.BeatWindowUnlock += OnBeatTick;
		Emitting = false;
		EmitParticle(Transform, Vector3.Zero, new Color(1, 1, 1), new Color(1, 1, 1), 0);
		((StandardMaterial3D)DrawPass1.SurfaceGetMaterial(0)).Emission = new Color(GD.Randf(), GD.Randf(), GD.Randf());
	}

	public override void _ExitTree()
	{
		Music.Singleton.BeatWindowUnlock -= OnBeatTick;
	}

	public void OnBeatTick(BeatTime time)
	{
		EmitParticle(Transform, Vector3.Zero, new Color(1, 1, 1), new Color(1, 1, 1), 0);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("RandomizeColor"))
		{
			((StandardMaterial3D)DrawPass1.SurfaceGetMaterial(0)).Emission = new Color(GD.Randf(), GD.Randf(), GD.Randf());
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
