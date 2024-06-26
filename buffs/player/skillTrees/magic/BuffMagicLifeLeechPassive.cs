using System;

namespace Project;

public partial class BuffMagicLifeLeechPassive : BaseBuff
{
	public const float LifeLeech = 0.1f;
	public BuffMagicLifeLeechPassive()
	{
		Settings = new()
		{
			FriendlyName = "Vampiric Essence",
			Description = MakeDescription(
				$"Regain {{{LifeLeech * 100}%}} of all damage dealt as Health.",
				$"\n\n((Life leech is a reaction, and will not trigger other reactions.))"
			),
			IconPath = "res://assets/icons/SpellBook06_105.png",
			Hidden = true,
		};
	}

	public override void ModifyUnit(BuffUnitStatsVisitor unit)
	{
		unit.LifeLeechFraction += LifeLeech;
	}
}