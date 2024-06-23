namespace Project;

public partial class BuffHalfMoveSpeed : BaseBuff
{
	public BuffHalfMoveSpeed()
	{
		Settings = new()
		{
			Description = "Your movespeed is halved.",
			Hidden = true,
		};
	}

	public override void ModifyUnit(BuffUnitStatsVisitor unit)
	{
		unit.MoveSpeedPercentage *= 0.5f;
	}
}