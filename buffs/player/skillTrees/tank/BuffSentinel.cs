using System;
using Godot;
using Project;

namespace Project;

public partial class BuffSentinel : BaseBuff
{
	public const float DamageReduction = 0.75f;
	public const float EffectDuration = 8;

	public BuffSentinel()
	{
		Settings = new()
		{
			FriendlyName = "Sentinel",
			DynamicDesc = () => MakeDescription(
				$"Increases your damage reduction by {{{Math.Round(DamageReduction * Stacks * 100) + "%"}}}."
			),
			IconPath = "res://assets/icons/SpellBook06_78.png",
		};
		Duration = EffectDuration;
	}

	public override void ModifyUnit(BuffUnitStatsVisitor unit)
	{
		unit.PercentageDamageTaken[ObjectResourceType.Health] -= DamageReduction;
	}
}