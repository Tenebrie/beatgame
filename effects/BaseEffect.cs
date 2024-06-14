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

	public void Attach(Node3D parentNode, Vector3 offset)
	{
		GetParent()?.RemoveChild(this);
		parentNode.AddChild(this);
		GlobalPosition = parentNode.GlobalPosition + offset;
	}

	public async void AttachForDuration(Node3D parentNode, float seconds, Vector3 offset)
	{
		parentNode.AddChild(this);
		GlobalPosition = parentNode.GlobalPosition + offset;
		await ToSignal(GetTree().CreateTimer(seconds), "timeout");
		parentNode.RemoveChild(this);
		parentNode.GetTree().Root.AddChild(this);
		GlobalPosition = parentNode.GlobalPosition + offset;
	}

	public async void CleanUp()
	{
		Emitter.Emitting = false;
		await ToSignal(GetTree().CreateTimer(4), "timeout");
		QueueFree();
	}
}
