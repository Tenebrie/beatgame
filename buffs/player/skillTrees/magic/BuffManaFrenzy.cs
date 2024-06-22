using System;

namespace Project;

public partial class BuffManaFrenzy : BaseBuff
{
	public const float DamageIncrease = 0.01f;
	public const float FrenzyDuration = 8;
	public BuffManaFrenzy()
	{
		Settings = new()
		{
			Description = $"Increases damage by {Colors.Tag(Math.Round(DamageIncrease * 100) + "%")}",
		};
		Duration = FrenzyDuration;
	}

	public override void ModifyOutgoingDamage(BuffOutgoingDamageVisitor damage)
	{
		damage.Value += damage.Value * DamageIncrease;
	}
}