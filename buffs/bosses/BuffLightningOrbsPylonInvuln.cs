using System;
using Project;

namespace Project;

public partial class BuffLightningOrbsPylonInvuln : BaseBuff
{
	public BuffLightningOrbsPylonInvuln()
	{
		Settings = new()
		{
			FriendlyName = "Storm Protection",
			Description = MakeDescription("Invulnerable to everything other than the {{power of the storm}} itself."),
			IconPath = "res://assets/icons/SpellBook06_119.PNG",
		};
	}

	public override void ModifyUnit(BuffUnitStatsVisitor visitor)
	{
		visitor.PercentageCCReduction = 1.00f;
	}

	public override void ModifyIncomingDamage(BuffIncomingDamageVisitor damage)
	{
		var buffCount = Math.Min(damage.SourceUnit.Buffs.Stacks<BuffPowerUpLightningOrb>(), (int)Math.Round(Parent.Health.Current));

		damage.Value = buffCount;
		damage.SourceUnit.Buffs.RemoveStacks<BuffPowerUpLightningOrb>(buffCount);
	}
}