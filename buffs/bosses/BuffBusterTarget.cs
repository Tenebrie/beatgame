namespace Project;

public partial class BuffBusterTarget : BaseBuff
{
	public BuffBusterTarget(BaseCast source)
	{

	}

	public override void Visit(BuffVisitor visitor)
	{
		// visitor.GravityModifier = 0;
	}
}