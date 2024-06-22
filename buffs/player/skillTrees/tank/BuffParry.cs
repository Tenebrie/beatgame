using System;
using System.Collections.Generic;
using Godot;
using Project;

namespace Project;

public partial class BuffParry : BaseBuff
{
	public const float RetaliateDamageFraction = 1.0f;

	public readonly Dictionary<BaseUnit, float> StoredDamage = new();

	public BuffParry()
	{
		Settings = new()
		{
			FriendlyName = "Parry",
			Description = MakeDescription(
				$"Block all damage, releasing {{{Math.Round(RetaliateDamageFraction * 100) + "%"}}} of it back to the attacker when the cast finishes."
			),
			IconPath = "res://assets/icons/SpellBook06_123.png",
		};
	}

	public override void ReactToIncomingDamage(BuffIncomingDamageVisitor damage)
	{
		StoredDamage[damage.SourceUnit] = StoredDamage.GetValueOrDefault(damage.SourceUnit) + damage.Value * RetaliateDamageFraction;
	}

	public override void ModifyUnit(BuffUnitStatsVisitor visitor)
	{
		visitor.PercentageDamageTaken[ObjectResourceType.Health] = 0;
	}
}