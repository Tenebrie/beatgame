using System;

namespace Project;

public partial class BuffMagicLifeLeechActive : BaseBuff
{
	public const float LifeLeech = 0.9f;
	public const float EffectDuration = 4;
	public BuffMagicLifeLeechActive()
	{
		Settings = new()
		{
			FriendlyName = "All Consuming Flame",
			Description = $"Regain {{{LifeLeech * 100}%}} of all damage dealt as Health.",
			IconPath = "res://assets/icons/SpellBook06_105.png",
		};
		Duration = EffectDuration;
	}

	public override void ModifyUnit(BuffUnitStatsVisitor unit)
	{
		unit.LifeLeechFraction += LifeLeech;
	}
}