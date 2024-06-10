namespace Project;

public partial class BuffPlus10Mana : BaseBuff
{
	public BuffPlus10Mana()
	{
		Settings = new()
		{
			Description = $"Increase your maximum Mana by [color={Colors.Mana}]{10}[/color]"
		};
	}
	public override void ModifyUnit(BuffUnitStatsVisitor unit)
	{
		unit.MaximumMana += 10;
	}
}