using System;
using Godot;
using Project;

namespace Project;

public partial class BuffSentinel : BaseBuff
{
	public const float DamageReduction = 0.5f;
	public const float EffectDuration = 4;

	public BuffSentinel()
	{
		Settings = new()
		{
			FriendlyName = "Sentinel",
			Description = MakeDescription(
				$"Increases your damage reduction by {{{Math.Round(DamageReduction * 100) + "%"}}}."
			),
		};
		Duration = EffectDuration;
	}

	public override void ModifyUnit(BuffUnitStatsVisitor unit)
	{
		unit.PercentageDamageTaken[ObjectResourceType.Health] *= DamageReduction;
	}
}