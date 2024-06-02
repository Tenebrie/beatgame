using Godot;
using System;
using System.Linq;

namespace Project;

public partial class AerialFaithPlateTarget : Node3D
{
	GroundAreaCircle circle;

	public override void _Ready()
	{
		circle = GetNode<GroundAreaCircle>("GroundAreaCircle");
		circle.QueueFree();
	}
}
