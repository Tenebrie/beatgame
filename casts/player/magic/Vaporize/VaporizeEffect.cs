using Godot;
using Project;
using System;

namespace Project;

public partial class VaporizeEffect : BaseEffect
{
	[Export] GpuParticles3D Fire;
	[Export] GpuParticles3D Smoke;
	[Export] GpuParticles3D Embers;

	public GroundAreaCircle Circle;

	public override void _Ready()
	{
		Fire.Restart();
		Smoke.Restart();
		Embers.Restart();
		FreeAfterDelayAsync();
	}
}
