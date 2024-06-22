namespace Project;

public partial class BuffTankTreeHealth : BaseBuff
{
	public BuffTankTreeHealth()
	{
		Settings = new()
		{
			Description = $"Increase your maximum Health by [color={Colors.Health}]{25}[/color]."
		};
	}

	public override void ModifyUnit(BuffUnitStatsVisitor unit)
	{
		unit.MaximumHealth += 25;
	}
}