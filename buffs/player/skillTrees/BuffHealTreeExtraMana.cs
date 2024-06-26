namespace Project;

public partial class BuffHealTreeExtraMana : BaseBuff
{
	public BuffHealTreeExtraMana()
	{
		Settings = new()
		{
			Description = $"Every minor node in this tree increases increases your maximum Mana by [color={Colors.Mana}]{10}[/color].",
			Hidden = true,
		};
	}
	public override void ModifyUnit(BuffUnitStatsVisitor unit)
	{
		unit.MaximumMana += 10;
	}
}