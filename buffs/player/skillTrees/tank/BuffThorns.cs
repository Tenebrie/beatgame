using System;
using Godot;
using Project;

namespace Project;

public partial class BuffThorns : BaseBuff
{
	public const float RetaliateDamageFraction = 0.1f;

	public BuffThorns()
	{
		Settings = new()
		{
			FriendlyName = "Thorns",
			Description = MakeDescription(
				$"Whenever you take damage, deal {{{Math.Round(RetaliateDamageFraction * 100) + "%"}}} of it back to the attacker."
			),
			Hidden = true,
		};
	}

	public override void ModifyUnit(BuffUnitStatsVisitor unit)
	{
		unit.RetaliateDamageFraction += RetaliateDamageFraction;
	}
}