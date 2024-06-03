using Godot;

namespace Project;

public partial class ProjectileImpact : Node3D
{
	public GpuParticles3D Emitter;

	public override void _Ready()
	{
		// GetNode<GpuParticles3D>("GPUParticles3D").OneShot = true;
		// GetNode<GpuParticles3D>("GPUParticles3D").Explosiveness = 1;
		Emitter = GetNode<GpuParticles3D>("GPUParticles3D");
		Emitter.Restart();
		Cleanup();
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

	private async void Cleanup()
	{
		await ToSignal(GetTree().CreateTimer(4), "timeout");
		QueueFree();
	}
}
