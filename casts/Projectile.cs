using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Project;

public partial class Projectile : Node3D
{
	private bool IsEmitting = true;
	public BaseUnit TargetUnit;
	public List<GpuParticles3D> Emitters;
	public float ImpactDamage = 5;

	PackedScene scene = GD.Load<PackedScene>("res://effects/FireballProjectile/FireballProjectileImpact.tscn");

	public override void _Ready()
	{
		Emitters = GetChildren().Where(child => child is GpuParticles3D).Cast<GpuParticles3D>().ToList();
	}

	public override void _Process(double delta)
	{
		if (!IsInstanceValid(TargetUnit))
		{
			Cleanup();
			return;
		}

		var speed = 6f * (float)delta;
		var targetPos = TargetUnit.GlobalPosition + new Vector3(0, 0.5f, 0);
		var direction = GlobalPosition.DirectionTo(targetPos);

		Position += direction * speed;
		if (GlobalPosition.DistanceSquaredTo(targetPos) <= 0.02f && IsEmitting)
		{
			Cleanup();

			var impact = scene.Instantiate() as ProjectileImpact;
			GetTree().Root.AddChild(impact);
			impact.GlobalPosition = GlobalPosition;

			TargetUnit.Health.Damage(ImpactDamage);
		}
	}

	private async void Cleanup()
	{
		IsEmitting = false;
		foreach (var emitter in Emitters)
		{
			emitter.Emitting = false;
		}
		await ToSignal(GetTree().CreateTimer(2), "timeout");
		QueueFree();
	}
}