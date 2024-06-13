namespace Project;

public partial class BuffExtraTankySummons : BaseBuff
{
	public const float HealthPerStack = 100;
	public BuffExtraTankySummons()
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