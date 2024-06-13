namespace Project;

public partial class BuffPlus10SummonHealth : BaseBuff
{
	public const float HealthPerStack = 10;
	public BuffPlus10SummonHealth()
	{
		Settings = new()
		{
			Description = $"Increase your summons' Health by [color={Colors.Health}]{HealthPerStack}[/color]."
		};
	}

	public override void ModifyUnit(BuffUnitStatsVisitor unit)
	{
		unit.SummonHealth += HealthPerStack;
	}
}