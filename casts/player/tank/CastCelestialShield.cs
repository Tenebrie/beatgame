using System;
using Godot;

namespace Project;

public partial class CastCelestialShield : BaseCast
{
	public const float HealthThreshold = 50;

	public CastCelestialShield(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Celestial Shield",
			Description = MakeDescription(
				$"A short breather is all you need.",
				$"For the next {{{BuffCelestialShield.EffectDuration}}} beats,",
				$"you are completely invulnerable, immune to movement effects and do not consume Mana for any casts."
			),
			IconPath = "res://assets/icons/SpellBook06_12.png",
			InputType = CastInputType.AutoRelease,
			TargetType = CastTargetType.None,
			CastTimings = BeatTime.Free,
			HoldTime = 2,
			RecastTime = 128,
			GlobalCooldown = true,
			CooldownOnCancel = false,
		};
	}

	protected override void OnCastCompleted(CastTargetData _)
	{
		Parent.Buffs.Add(new BuffJuggernaut()
		{
			ExtraHealthRegen = Parent.Health.Maximum / 250,
			SourceCast = this,
		});
	}
}