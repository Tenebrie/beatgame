using System;
using Godot;
using Project;

namespace Project;

public partial class BuffJuggernaut : BaseBuff
{
	public float ExtraHealthRegen;
	public const float EffectDuration = 8;

	public BuffJuggernaut()
	{
		Settings = new()
		{
			FriendlyName = "Juggernaut",
			Description = MakeDescription(
				$"Increases your base Health regeneration by {{{Math.Round(ExtraHealthRegen)}}} per beat."
			),
		};
		Duration = EffectDuration;
	}

	public override void ModifyUnit(BuffUnitStatsVisitor unit)
	{
		unit.FlatResourceRegen[ObjectResourceType.Health] += ExtraHealthRegen;
	}
}