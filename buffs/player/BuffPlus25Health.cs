namespace Project;

public partial class BuffPlus25Health : BaseBuff
{
	public override void ModifyUnit(BuffUnitStatsVisitor unit)
	{
		unit.MaximumHealth += 25;
	}
}