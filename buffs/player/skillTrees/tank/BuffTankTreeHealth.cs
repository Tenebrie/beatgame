namespace Project;

public partial class BuffTankTreeHealth : BaseBuff
{
	public BuffTankTreeHealth()
	{
		Settings = new()
		{
			Description = $"Every minor node in this tree increases your maximum Health by [color={Colors.Health}]{25}[/color].",
			Hidden = true,
		};
	}

	public override void ModifyUnit(BuffUnitStatsVisitor unit)
	{
		unit.MaximumHealth += 25;
	}
}