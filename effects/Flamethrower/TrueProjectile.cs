using System.Linq;
using Godot;

namespace Project;

public partial class TrueProjectile : Node3D
{
	public UnitAlliance Alliance;
	public GpuParticles3D Emitter;

	public override void _Ready()
	{
		Emitter = GetNode<GpuParticles3D>("GPUParticles3D");
		ForceCleanup();
	}

	public override void _Process(double delta)
	{
		var speed = 7f * (float)delta;

		var forward = -GlobalTransform.Basis.Z;
		Position += forward * speed;

		if (!Emitter.Emitting)
		{
			return;
		}

		var hitUnits = BaseUnit.AllUnits.Where(unit => unit.Alliance.HostileTo(Alliance) && unit.GlobalPosition.DistanceSquaredTo(GlobalPosition) <= 0.5f).ToList();
		if (hitUnits.Count == 0)
		{
			return;
		}

		var targetUnit = hitUnits[0];
		targetUnit.Health.Damage(1);
		Cleanup();

		// var scene = GD.Load<PackedScene>("res://effects/FireballProjectile/FireballProjectileImpact.tscn");
		// var impact = scene.Instantiate() as ProjectileImpact;
		// GetTree().Root.AddChild(impact);
		// impact.GlobalPosition = GlobalPosition;

	}

	private async void ForceCleanup()
	{
		await ToSignal(GetTree().CreateTimer(2), "timeout");
		QueueFree();
	}

	private async void Cleanup()
	{
		Emitter.Emitting = false;
		await ToSignal(GetTree().CreateTimer(2), "timeout");
		QueueFree();
	}
}