namespace Project;

public partial class BuffTridentTargetSlow : BaseBuff
{
	public BuffTridentTargetSlow()
	{
		Duration = 8f;
	}
	public override void ModifyUnit(BuffUnitStatsVisitor visitor)
	{
		visitor.MoveSpeedPercentage *= 0.5f;
	}
}