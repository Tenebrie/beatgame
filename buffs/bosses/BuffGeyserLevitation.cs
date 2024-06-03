namespace Project;

public partial class BuffGeyserLevitation : BaseBuff
{
	public override void ModifyUnit(BuffUnitStatsVisitor visitor)
	{
		visitor.GravityModifier = 0;
	}
}