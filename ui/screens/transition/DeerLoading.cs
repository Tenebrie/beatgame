using Godot;
using System;

namespace Project;

[Tool]
public partial class DeerLoading : AnimatedSprite2D
{
	Control parent;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		parent = GetParent<Control>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position = parent.Position + parent.Size / 2;
	}
}
