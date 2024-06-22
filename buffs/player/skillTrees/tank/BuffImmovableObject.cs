using System;
using Godot;
using Project;

namespace Project;

public partial class BuffImmovableObject : BaseBuff
{
	public const float EffectPower = 1.0f;
	public const float EffectDuration = 4;

	public BuffImmovableObject()
	{
		Settings = new()
		{
			Description = MakeDescription(
				$"Increases your crowd control resistance by {{{Math.Round(EffectPower * 100) + "%"}}}."
			),
		};
		Duration = EffectDuration;
		// if (!SourceCast.HasSkill<EquivalentMobility>())
		// {
		Settings.Description += $"\nAlso reduces your movement speed to {{0}}.";
		// }
	}

	public override void ModifyUnit(BuffUnitStatsVisitor unit)
	{
		unit.PercentageCCReduction += EffectPower;
		// If 
		// if (!SourceCast.HasSkill<EquivalentMobility>())
		// {
		unit.MoveSpeedPercentage = 0;
		// }
	}
}