using Godot;

namespace Project;
public class Fireball : BaseCast
{
	static readonly PackedScene scene = GD.Load<PackedScene>("res://effects/FireballProjectile/FireballProjectile.tscn");

	public Fireball(BaseUnit parent) : base(parent)
	{
		TargetType = CastTargetType.Unit;
		TargetAlliances = new() { UnitAlliance.Hostile };
		CastTimings = BeatTime.One;
		RecastTime = 0;
	}

	protected override void CastOnUnit(BaseUnit target)
	{
		var fireball = scene.Instantiate() as Projectile;
		Parent.GetTree().Root.AddChild(fireball);
		fireball.GlobalPosition = Parent.GlobalPosition + new Vector3(0, 0.5f, 0);
		fireball.TargetUnit = target;
	}
}