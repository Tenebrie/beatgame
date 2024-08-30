using System;
using Godot;

namespace Project;

public partial class CastSentinel : BaseCast
{
	public const float ManaCost = 85;

	public CastSentinel(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Sentinel",
			Description = MakeDescription(
				$"Prepare your defenses for an incoming attack.",
				$"For the next {{{BuffSentinel.EffectDuration}}} beats,",
				$"you gain {{{Math.Round(BuffSentinel.DamageReduction * 100) + "%"}}} damage reduction."
			),
			IconPath = "res://assets/icons/SpellBook06_78.png",
			InputType = CastInputType.Instant,
			TargetType = CastTargetType.None,
			RecastTime = 32,
			Charges = 1,
			GlobalCooldown = GlobalCooldownMode.Ignore,
		};

		if (this.HasSkill<SkillSentinelCharges>())
		{
			Settings.Charges = 2;
		}
		if (this.HasSkill<SkillSentinelMana>())
		{
			Settings.RecastTime = 1;
			Settings.ResourceCost[ObjectResourceType.Mana] = ManaCost;
		}
	}

	protected override void OnCastCompleted(CastTargetData _)
	{
		Parent.Buffs.Add(new BuffSentinel()
		{
			SourceCast = this,
		});
	}
}