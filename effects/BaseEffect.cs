using Godot;
using System;

namespace Project;

public partial class BaseEffect : Node3D
{
	[Export]
	public GpuParticles3D Emitter;

	public override void _Ready()
	{
		Emitter = GetNode<GpuParticles3D>("GPUParticles3D");
	}

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

	// public BaseEffect AttachForDuration(Node3D parentNode, float seconds)
	// {
	// 	parentNode.AddChild(this);
	// 	DetachAfterDelayAsync(parentNode, seconds);
	// 	return this;
	// }

	// public BaseEffect AttachForDuration(Node3D parentNode, float seconds, Vector3 offset)
	// {
	// 	parentNode.AddChild(this);
	// 	GlobalPosition = parentNode.GlobalPosition + offset;
	// 	DetachAfterDelayAsync(parentNode, seconds);
	// 	return this;
	// }

	public BaseEffect Stop()
	{
		Emitter.Emitting = false;
		return this;
	}

	public BaseEffect SetLifetime(float seconds)
	{
		StopAfterDelayAsync(seconds);
		FreeAfterDelayAsync(seconds + 4);
		return this;
	}

	public BaseEffect FreeAfterDelay(float seconds = 4)
	{
		FreeAfterDelayAsync(seconds);
		return this;
	}

	private async void StopAfterDelayAsync(float seconds)
	{
		await ToSignal(GetTree().CreateTimer(seconds), "timeout");
		Stop();
	}

	private async void FreeAfterDelayAsync(float seconds = 4)
	{
		await ToSignal(GetTree().CreateTimer(seconds), "timeout");
		QueueFree();
	}

	private async void DetachAfterDelayAsync(Node3D parentNode, float seconds)
	{
		await ToSignal(GetTree().CreateTimer(seconds), "timeout");
		parentNode.RemoveChild(this);
		parentNode.GetTree().Root.AddChild(this);
	}
}
