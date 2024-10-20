using System;
using Godot;

namespace Project;

public partial class CastAllConsumingFlame : BaseCast
{
	public const float ManaCost = 20;

	public CastAllConsumingFlame(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "All Consuming Flame",
			Description = MakeDescription(
				$"Infuse your magic with an even more powerful vampiric essence.",
				$"For the next {{{BuffMagicLifeLeechActive.EffectDuration}}} beats,",
				$"you regain {{{BuffMagicLifeLeechActive.LifeLeech * 100}%}} of all damage dealt as Health.",
				$"\n\n((Life leech is a reaction, and will not trigger other reactions.))"
			),
			IconPath = "res://assets/icons/SpellBook06_105.png",
			InputType = CastInputType.Instant,
			TargetType = CastTargetType.None,
			RecastTime = 32,
			GlobalCooldown = GlobalCooldownMode.Ignore,
		};
		Settings.ResourceCost[ObjectResourceType.Mana] = ManaCost;
	}

	protected override void OnCastCompleted(CastTargetData _)
	{
		Parent.Buffs.Add(new BuffMagicLifeLeechActive()
		{
			SourceCast = this,
		});
	}
}