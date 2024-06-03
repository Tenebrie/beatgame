namespace Project;

public partial class BuffBusterTarget : BaseBuff
{
	public BuffBusterTarget(BaseCast source)
	{

	}

	public override void ModifyUnit(BuffUnitStatsVisitor visitor)
	{
		// visitor.GravityModifier = 0;
	}
}