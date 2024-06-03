namespace Project;

public partial class BuffPushBackLevitation : BaseBuff
{
	public override void ModifyUnit(BuffUnitStatsVisitor visitor)
	{
		visitor.GravityModifier = 0;
	}
}