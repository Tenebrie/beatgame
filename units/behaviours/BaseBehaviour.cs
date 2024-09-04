using System;
using System.Linq;
using Godot;

namespace Project;

public partial class BaseBehaviour : Node
{
	protected BaseUnit Parent
	{
		get
		{
			var parent = GetParent();
			if (parent is BaseUnit unit)
				return unit;
			throw new Exception("This behaviour is not a child of BaseUnit");
		}
	}

	protected Vector3 Position { get => Parent.Position; }
	protected Vector3 Velocity { get => Parent.Velocity; }
	protected Vector3 GlobalPosition { get => Parent.GlobalPosition; }
	protected Transform3D GlobalTransform { get => Parent.GlobalTransform; }
	protected T GetComponent<T>() where T : Node => Parent.GetComponent<T>();
}