using System;
using Godot;

namespace Project;

public partial class CastJuggernaut : BaseCast
{
	public const float HealthThreshold = 50;

	public CastJuggernaut(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Juggernaut",
			Description = MakeDescription(
				$"A short breather is all you need.",
				$"For the next {{{BuffJuggernaut.EffectDuration}}} beats,",
				$"your base Health regeneration is increased by {{{1}}} per beat for each {{{HealthThreshold}}} maximum Health you have."
			),
			IconPath = "res://assets/icons/SpellBook06_112.png",
			InputType = CastInputType.Instant,
			TargetType = CastTargetType.None,
			CastTimings = BeatTime.Free,
			RecastTime = 32,
			GlobalCooldown = true,
		};
		Settings.ResourceCost[ObjectResourceType.Mana] = 200;
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