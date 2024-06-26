namespace Project;

public partial class BuffTankTreeHealthRegen : BaseBuff
{
	public const float Regen = 0.1f;

	public BuffTankTreeHealthRegen()
	{
		Settings = new()
		{
			Description = MakeDescription($"Every minor node in this tree increases increases your Health regeneration by {{{Regen * 100}%}}."),
			IconPath = "res://assets/icons/SpellBook06_07.png",
			Hidden = true,
		};
	}

	public override void ModifyUnit(BuffUnitStatsVisitor unit)
	{
		unit.PercentageResourceRegen[ObjectResourceType.Health] += Regen;
	}
}