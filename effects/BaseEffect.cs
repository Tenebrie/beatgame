using Godot;
using System;

namespace Project;

public partial class BaseEffect : Node3D
{
	public BaseEffect Attach(Node3D parentNode)
	{
		return Attach(parentNode, Position - parentNode.Position);
	}

	public BaseEffect Attach(Node3D parentNode, Vector3 offset)
	{
		GetParent()?.RemoveChild(this);
		parentNode.AddChild(this);
		GlobalPosition = parentNode.GlobalPosition + offset;
		return this;
	}

	public BaseEffect AttachForDuration(Node3D parentNode, float seconds)
	{
		parentNode.AddChild(this);
		DetachAfterDelayAsync(parentNode, seconds);
		return this;
	}

	// public BaseEffect AttachForDuration(Node3D parentNode, float seconds, Vector3 offset)
	// {
	// 	parentNode.AddChild(this);
	// 	GlobalPosition = parentNode.GlobalPosition + offset;
	// 	DetachAfterDelayAsync(parentNode, seconds);
	// 	return this;
	// }

	public virtual BaseEffect SetLifetime(float seconds)
	{
		FreeAfterDelayAsync(seconds);
		return this;
	}

	public BaseEffect FreeAfterDelay(float seconds = 4)
	{
		FreeAfterDelayAsync(seconds);
		return this;
	}

	private async void DetachAfterDelayAsync(Node3D parentNode, float seconds)
	{
		await ToSignal(GetTree().CreateTimer(seconds), "timeout");
		var transform = GlobalTransform;
		parentNode.RemoveChild(this);
		parentNode.GetTree().CurrentScene.AddChild(this);
		GlobalTransform = transform;
	}


	protected async void FreeAfterDelayAsync(float seconds = 4)
	{
		await ToSignal(GetTree().CreateTimer(seconds), "timeout");
		QueueFree();
	}
}
