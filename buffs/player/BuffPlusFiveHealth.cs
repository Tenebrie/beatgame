namespace Project;

public partial class BuffPlusFiveHealth : BaseBuff
{
	public BuffPlusFiveHealth()
	{
		Settings = new()
		{
			Hidden = true,
		};
	}

	public override void ModifyUnit(BuffUnitStatsVisitor unit)
	{
		unit.MaximumHealth += 5;
	}
}