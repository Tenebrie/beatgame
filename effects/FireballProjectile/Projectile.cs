using Godot;

namespace Project;

public partial class Projectile : Node3D
{
	public BaseUnit TargetUnit;
	public GpuParticles3D Emitter;
	public float ImpactDamage = 5;

	public override void _Ready()
	{
		Emitter = GetNode<GpuParticles3D>("GPUParticles3D");
	}

	public override void _Process(double delta)
	{
		if (!IsInstanceValid(TargetUnit))
		{
			Cleanup();
			return;
		}

		var speed = 10f * (float)delta;
		var targetPos = TargetUnit.GlobalPosition + new Vector3(0, 0.5f, 0);
		var direction = GlobalPosition.DirectionTo(targetPos);

		Position += direction * speed;
		if (GlobalPosition.DistanceSquaredTo(targetPos) <= 0.02f && Emitter.Emitting)
		{
			Cleanup();

			var scene = GD.Load<PackedScene>("res://effects/FireballProjectile/FireballProjectileImpact.tscn");
			var impact = scene.Instantiate() as ProjectileImpact;
			GetTree().Root.AddChild(impact);
			impact.GlobalPosition = GlobalPosition;

			TargetUnit.Health.Damage(ImpactDamage);
		}
	}

	private async void Cleanup()
	{
		Emitter.Emitting = false;
		await ToSignal(GetTree().CreateTimer(2), "timeout");
		QueueFree();
	}
}