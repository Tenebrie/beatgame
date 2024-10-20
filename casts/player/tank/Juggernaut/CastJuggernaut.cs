using System;
using Godot;

namespace Project;

public partial class CastJuggernaut : BaseCast
{
	public const float HealthThreshold = 40;

	public CastJuggernaut(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Juggernaut",
			Description = MakeDescription(
				$"For the next {{{Buff.EffectDuration}}} beats,",
				$"your base Health regeneration is increased by {{{1}}} per beat for each {{{HealthThreshold}}} maximum Health you have."
			),
			LoreDescription = $"A short breather is all you need.",
			IconPath = "res://assets/icons/SpellBook06_112.png",
			InputType = CastInputType.Instant,
			TargetType = CastTargetType.None,
			RecastTime = 48,
		};
		Settings.ResourceCost[ObjectResourceType.Mana] = 80;
	}

	protected override void OnCastCompleted(CastTargetData _)
	{
		Parent.Buffs.Add(new Buff()
		{
			ExtraHealthRegen = Parent.Health.Maximum / HealthThreshold,
			SourceCast = this,
		});
	}

	public partial class Buff : BaseBuff
	{
		public float ExtraHealthRegen;
		public const float EffectDuration = 12;

		public Buff()
		{
			Settings = new()
			{
				FriendlyName = "Juggernaut",
				DynamicDesc = () => MakeDescription(
					$"Increases your base Health regeneration by {{{Math.Round(ExtraHealthRegen)}}} per beat."
				),
				IconPath = "res://assets/icons/SpellBook06_112.png",
			};
			Duration = EffectDuration;
		}

		public override void ModifyUnit(BuffUnitStatsVisitor unit)
		{
			unit.FlatResourceRegen[ObjectResourceType.Health] += ExtraHealthRegen;
		}
	}
}