namespace Project;

public partial class BuffPushBackLevitation : BaseBuff
{
	public override void Visit(BuffVisitor visitor)
	{
		visitor.GravityModifier = 0;
	}
}