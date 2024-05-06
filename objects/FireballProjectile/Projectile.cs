using System.Diagnostics;
using Godot;
using Project;

namespace Project;

public partial class Projectile : Node3D
{
	public BaseUnit targetUnit;

	public override void _Process(double delta)
	{
		var speed = 10f * (float)delta;
		var targetPos = targetUnit.GlobalPosition + new Vector3(0, 0.5f, 0);
		var direction = GlobalPosition.DirectionTo(targetPos);

		Position += direction * speed;
		var emitter = GetNode<GpuParticles3D>("GPUParticles3D");
		if (GlobalPosition.DistanceSquaredTo(targetPos) <= 0.02f && emitter.Emitting)
		{
			emitter.Emitting = false;
			Cleanup();

			var scene = GD.Load<PackedScene>("res://objects/FireballProjectile/FireballProjectileImpact.tscn");
			var impact = scene.Instantiate() as ProjectileImpact;
			GetTree().Root.AddChild(impact);
			impact.GlobalPosition = GlobalPosition;
		}
	}

	private async void Cleanup()
	{
		await ToSignal(GetTree().CreateTimer(2), "timeout");
		QueueFree();
	}
}