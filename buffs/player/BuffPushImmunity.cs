namespace Project;

public partial class BuffPushImmunity : BaseBuff
{
	public BuffPushImmunity()
	{
		Settings = new()
		{
			Description = "Immune to push effects.",
			Hidden = true,
		};
	}

    public override void ModifyUnit(BuffUnitStatsVisitor unit)
    {
        unit.PercentageCCReduction = 1;
    }
}