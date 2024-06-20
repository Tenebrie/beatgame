using System;

namespace Project;

public partial class BuffMagicTreeManaRegen : BaseBuff
{
	public const float Regen = 0.05f;

	public BuffMagicTreeManaRegen()
	{
		Settings = new()
		{
			Description = $"Increase your Mana regeneration by [color={Colors.Mana}]{Math.Round(Regen * 100) + "%"}[/color]."
		};
	}
	public override void ModifyUnit(BuffUnitStatsVisitor unit)
	{
		unit.PercentageResourceRegen[ObjectResourceType.Mana] += Regen;
	}
}