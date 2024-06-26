using System;

namespace Project;

public partial class BuffMagicTreeManaRegen : BaseBuff
{
	public const float Regen = 0.075f;

	public BuffMagicTreeManaRegen()
	{
		Settings = new()
		{
			Description = $"Every minor node in this tree increases your Mana regeneration by [color={Colors.Mana}]{Math.Round(Regen * 100, 1) + "%"}[/color].",
			Hidden = true,
		};
	}
	public override void ModifyUnit(BuffUnitStatsVisitor unit)
	{
		unit.PercentageResourceRegen[ObjectResourceType.Mana] += Regen;
	}
}