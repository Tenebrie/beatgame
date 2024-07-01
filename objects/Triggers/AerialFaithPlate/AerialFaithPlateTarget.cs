using Godot;
using System;
using System.Linq;

namespace Project;

public partial class AerialFaithPlateTarget : Node3D
{
	CircularTelegraph circle;

	public override void _Ready()
	{
		circle = GetNode<CircularTelegraph>("GroundAreaCircle");
		circle.QueueFree();
	}
}
