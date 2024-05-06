using Godot;

namespace Project;

public abstract partial class BaseUnit : CharacterBody3D
{
	public Alliance alliance = Alliance.Neutral;
	public ObjectTargeting targeting = new();

	public override void _Ready()
	{
		targeting.Ready(this);
	}

	public override void _Process(double delta)
	{
		targeting.Process(delta);
	}

	public override void _Input(InputEvent @event)
	{
		targeting._Input(@event);
	}

	public override void _ExitTree()
	{
		targeting.ExitTree();
	}
}
