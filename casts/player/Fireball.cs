using Godot;

namespace Project;
public partial class Fireball : BaseCast
{
	public Fireball(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			InputType = CastInputType.AutoRelease,
			TargetType = CastTargetType.HostileUnit,
			TargetAlliances = new() { UnitAlliance.Hostile },
			CastTimings = BeatTime.One | BeatTime.Half,
			ReleaseTimings = BeatTime.One | BeatTime.Half,
			HoldTime = 1,
			RecastTime = 0,
		};
	}

	protected override void CastOnUnit(BaseUnit target)
	{
		var fireball = Lib.Scene(Lib.Effect.FireballProjectile).Instantiate() as Projectile;
		GetTree().Root.AddChild(fireball);
		fireball.GlobalPosition = Parent.GlobalPosition + new Vector3(0, 0.5f, 0);
		fireball.TargetUnit = target;
		var damage = Flags.CastSuccessful ? 10 : 5;
		fireball.ImpactDamage = damage;
	}
}