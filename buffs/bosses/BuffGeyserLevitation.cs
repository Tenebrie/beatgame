namespace Project;

public partial class BuffGeyserLevitation : BaseBuff
{
	public override void Visit(BuffVisitor visitor)
	{
		visitor.GravityModifier = 0;
	}
}