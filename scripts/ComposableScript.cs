using System.Collections.Generic;
using Godot;

namespace Project;

public abstract class ComposableScript
{
	public BaseUnit Parent;
	public List<ComposableScript> Composables = new();

	public ComposableScript(BaseUnit parent)
	{
		this.Parent = parent;
	}

	public virtual void _Ready()
	{
		foreach (var composable in Composables)
		{
			composable._Ready();
		}
	}

	public virtual void _Process(double delta)
	{
		foreach (var composable in Composables)
		{
			composable._Process(delta);
		}
	}

	public virtual void _Input(InputEvent @event)
	{
		foreach (var composable in Composables)
		{
			composable._Input(@event);
		}
	}

	public virtual void _ExitTree()
	{
		foreach (var composable in Composables)
		{
			composable._ExitTree();
		}
	}

	public SceneTree GetTree() => Parent.GetTree();
	public Window GetWindow() => Parent.GetWindow();
	public Vector3 Position { get => Parent.Position; }
	public Vector3 Velocity { get => Parent.Velocity; }
	public Vector3 GlobalPosition { get => Parent.GlobalPosition; }
	public Transform3D GlobalTransform { get => Parent.GlobalTransform; }
}