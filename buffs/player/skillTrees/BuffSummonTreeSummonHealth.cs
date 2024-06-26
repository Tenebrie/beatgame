namespace Project;

public partial class BuffSummonTreeSummonHealth : BaseBuff
{
	public const float HealthPerStack = 10;
	public BuffSummonTreeSummonHealth()
	{
		Settings = new()
		{
			Description = $"Every minor node in this tree increases your summons' Health by [color={Colors.Health}]{HealthPerStack}[/color].",
			Hidden = true,
		};
	}

	public override void ModifyUnit(BuffUnitStatsVisitor unit)
	{
		unit.SummonHealth += HealthPerStack;
	}
}