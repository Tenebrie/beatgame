namespace Project;

public partial class BuffTankTreeHealth : BaseBuff
{
	public override void ModifyUnit(BuffUnitStatsVisitor unit)
	{
		unit.MaximumHealth += 25;
	}
}