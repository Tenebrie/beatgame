using Godot;

namespace Project;
public partial class Fireball : BaseCast
{
	public Fireball(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Fireball",
			IconPath = "res://assets/icons/SpellBook06_15.PNG",
			InputType = CastInputType.AutoRelease,
			TargetType = CastTargetType.HostileUnit,
			CastTimings = BeatTime.Whole | BeatTime.Half,
			HoldTime = 2,
			RecastTime = 0,
		};
	}

	protected override void OnCastCompleted(CastTargetData target)
	{
		var fireball = Lib.Scene(Lib.Effect.FireballProjectile).Instantiate() as Projectile;
		GetTree().Root.AddChild(fireball);
		fireball.GlobalPosition = Parent.GlobalPosition + new Vector3(0, 0.5f, 0);
		fireball.Source = this;
		fireball.TargetUnit = target.HostileUnit;
		var damage = Flags.CastSuccessful ? 10 : 5;
		fireball.ImpactDamage = damage;
	}
}