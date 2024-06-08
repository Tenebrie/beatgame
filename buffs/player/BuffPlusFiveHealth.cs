namespace Project;

public partial class BuffPlusFiveHealth : BaseBuff
{
	public override void ModifyUnit(BuffUnitStatsVisitor unit)
	{
		unit.MaximumHealth += 5;
	}
}