using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Project;

public partial class Projectile : Node3D
{
	private bool IsEmitting = true;
	public BaseCast Source;
	public BaseUnit TargetUnit;
	public List<GpuParticles3D> Emitters;
	public float ImpactDamage = 5;

	public override void _Ready()
	{
		Emitters = GetChildren().Where(child => child is GpuParticles3D).Cast<GpuParticles3D>().ToList();
	}

	public override void _Process(double delta)
	{
		if (TargetUnit == null)
			return;

		if (!IsInstanceValid(TargetUnit))
		{
			CleanUp();
			return;
		}

		var speed = 6f * (float)delta;
		var targetPos = TargetUnit.GlobalPosition + new Vector3(0, 0.5f, 0);
		var direction = GlobalPosition.DirectionTo(targetPos);

		Position += direction * speed;
		if (GlobalPosition.DistanceSquaredTo(targetPos) <= 0.02f && IsEmitting)
		{
			CleanUp();

			var impact = Lib.LoadScene(Lib.Effect.FireballProjectileImpact).Instantiate() as ProjectileImpact;
			GetTree().Root.AddChild(impact);
			impact.GlobalPosition = GlobalPosition;

			if (Source == null)
				GD.PushError("Projectile doesn't have Source set");

			TargetUnit.Health.Damage(ImpactDamage, Source);
		}
	}

	public async void CleanUp()
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