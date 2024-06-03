using System;
using Project;

namespace Project;

public partial class BuffLightningOrbsPylonInvuln : BaseBuff
{
	public override void ModifyUnit(BuffUnitStatsVisitor visitor)
	{
		visitor.PercentageCCReduction = 1.00f;
	}

	public override void ModifyIncomingDamage(BuffIncomingDamageVisitor damage)
	{
		var buffCount = Math.Min(damage.SourceCast.Parent.Buffs.Stacks<BuffPowerUpLightningOrb>(), (int)Math.Round(Parent.Health.Current));

		damage.Value = buffCount;
		damage.SourceCast.Parent.Buffs.RemoveStacks<BuffPowerUpLightningOrb>(buffCount);
	}
}