using Project;

namespace Project;

public partial class BuffPowerUpLightningOrb : BaseBuff
{
	public override void ModifyUnit(BuffUnitStatsVisitor unit)
	{
		unit.MoveSpeedPercentage += 0.1f;
	}
}