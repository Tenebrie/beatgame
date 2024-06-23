using System.Collections.Generic;
using Godot;

namespace Project;

public partial class ComposableScript : Node
{
	public BaseUnit Parent;

	public ComposableScript(BaseUnit parent)
	{
		this.Parent = parent;
	}

	public Vector3 Position { get => Parent.Position; }
	public Vector3 Velocity { get => Parent.Velocity; }
	public Vector3 GlobalPosition { get => Parent.GlobalPosition; }
	public Transform3D GlobalTransform { get => Parent.GlobalTransform; }
}