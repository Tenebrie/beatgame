namespace Project;

public partial class BuffHealTreeExtraMana : BaseBuff
{
	public BuffHealTreeExtraMana()
	{
		Settings = new()
		{
			Description = $"Increase your maximum Mana by [color={Colors.Mana}]{10}[/color]."
		};
	}
	public override void ModifyUnit(BuffUnitStatsVisitor unit)
	{
		unit.MaximumMana += 10;
	}
}