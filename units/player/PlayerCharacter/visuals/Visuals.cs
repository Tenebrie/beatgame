using Godot;
using System;

public partial class Visuals : Node3D
{
	float Time;
	float VerticalOffset;
	public override void _Ready()
	{
		
	}

	public override void _Process(double delta)
	{
		Time += (float)delta;
		float verticalOffset = (float)Math.Sin(Time * 2) / 25;
		Position = new Vector3(Position.X, Position.Y - VerticalOffset + verticalOffset, Position.Z);
		VerticalOffset = verticalOffset;
	}

	public StandardMaterial3D GetMaterial()
	{
		return GetNode<GpuParticles3D>("OnBeatEmitterLocalSpace").DrawPass1.SurfaceGetMaterial(0) as StandardMaterial3D;
	}
}
