using Godot;

namespace Project;
public partial class Fireblast : BaseCast
{
	public Fireblast(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			InputType = CastInputType.Instant,
			TargetType = CastTargetType.HostileUnit,
			TargetAlliances = new() { UnitAlliance.Hostile },
			CastTimings = BeatTime.One | BeatTime.Half,
			RecastTime = 0.1f,
		};
	}

	protected override void CastOnUnit(BaseUnit target)
	{
		var impact = Lib.Scene(Lib.Effect.FireballProjectileImpact).Instantiate() as ProjectileImpact;
		GetTree().Root.AddChild(impact);
		impact.GlobalPosition = Parent.GlobalPosition;

		var damage = 5f;
		target.Health.Damage(damage);
	}
}