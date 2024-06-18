using Godot;

namespace Project;
public partial class CastIgnite : BaseCast
{
	public const float InstantDamage = 5;
	public CastIgnite(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Ignite",
			Description = MakeDescription(
				$"Surround your enemy by persistent flames dealing {{{InstantDamage}}} Fire damage instantly",
				$"and {{{BuffIgnite.DamagePerBeat}}} Fire damage per beat for {{{BuffIgnite.BurnDuration}}} beats.",
				$"\n\n((This effect does not stack.))"
			),
			IconPath = "res://assets/icons/SpellBook06_29.PNG",
			InputType = CastInputType.Instant,
			TargetType = CastTargetType.HostileUnit,
			CastTimings = BeatTime.Quarter | BeatTime.Eighth,
			RecastTime = 1,
		};
		Settings.ResourceCost[ObjectResourceType.Mana] = 20;
	}

	protected override void OnCastCompleted(CastTargetData target)
	{
		var impact = Lib.LoadScene(Lib.Effect.FireballProjectileImpact).Instantiate() as ProjectileImpact;
		GetTree().Root.AddChild(impact);
		impact.GlobalPosition = target.HostileUnit.GlobalCastAimPosition;

		var damage = 5f;
		target.HostileUnit.Health.Damage(damage, this);
		target.HostileUnit.Buffs.Add(new BuffIgnite()
		{
			SourceCast = this,
		});
	}
}