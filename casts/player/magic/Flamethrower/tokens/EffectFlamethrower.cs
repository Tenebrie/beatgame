using Godot;
using System;

namespace Project;

public partial class EffectFlamethrower : Node3D
{
	public GpuParticles3D Fire;
	public GpuParticles3D Smoke;
	public GpuParticles3D Embers;

	public override void _Ready()
	{
		Fire = GetNode<GpuParticles3D>("Fire");
		Smoke = GetNode<GpuParticles3D>("Smoke");
		Embers = GetNode<GpuParticles3D>("Embers");
	}
}
