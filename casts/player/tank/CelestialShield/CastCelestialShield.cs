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
				$"For the next {{{Buff.EffectDuration}}} beats,",
				$"you are completely invulnerable, immune to movement effects and do not consume Mana for any casts."
			),
			LoreDescription = "Invoke the power of the celestial.",
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
		Parent.Buffs.Add(new Buff()
		{
			SourceCast = this,
		});
	}

	public partial class Buff : BaseBuff
	{
		public const float EffectDuration = 8;

		public Buff()
		{
			Settings = new()
			{
				FriendlyName = "Celestial Shield",
				Description = MakeDescription(
					$"You are completely invulnerable."
				),
				IconPath = "res://assets/icons/SpellBook06_12.png",
			};
			Duration = EffectDuration;
		}

		public override void ModifyUnit(BuffUnitStatsVisitor visitor)
		{
			visitor.PercentageDamageTaken[ObjectResourceType.Health] = 0;
			visitor.PercentageDamageTaken[ObjectResourceType.Mana] = 0;
			visitor.PercentageCCReduction = 1;
		}
	}
}