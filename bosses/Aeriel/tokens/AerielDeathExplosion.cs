using Godot;
using Project;
using System;

namespace Project;

public partial class AerielDeathExplosion : Node3D
{
	[Export] GpuParticles3D Explosion;

	public override void _Ready()
	{
		Explosion.Restart();
	}
}
