using Godot;

namespace Project;
public partial class Fireball : BaseCast
{
	float Damage = 10;
	public Fireball(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Fireball",
			Description = $"Shoots out a fiery projectile that will always hit the target, dealing [color=orange]{Damage}[/color] Fire damage.",
			IconPath = "res://assets/icons/SpellBook06_15.PNG",
			InputType = CastInputType.AutoRelease,
			TargetType = CastTargetType.HostileUnit,
			CastTimings = BeatTime.Whole | BeatTime.Half,
			HoldTime = 2,
			RecastTime = 2,
		};
		Settings.ResourceCost[ObjectResourceType.Mana] = 10;
	}

	protected override void OnCastCompleted(CastTargetData target)
	{
		var fireball = Lib.Scene(Lib.Effect.FireballProjectile).Instantiate() as Projectile;
		GetTree().Root.AddChild(fireball);
		fireball.GlobalPosition = Parent.GlobalPosition + new Vector3(0, 0.5f, 0);
		fireball.Source = this;
		fireball.TargetUnit = target.HostileUnit;
		var damage = Flags.CastSuccessful ? Damage : 5;
		fireball.ImpactDamage = damage;
	}
}