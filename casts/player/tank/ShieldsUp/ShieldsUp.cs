using System;
using Godot;

namespace Project;
public partial class ShieldsUp : BaseCast
{
	public ShieldsUp(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Shields Up",
			Description = MakeDescription(
				$"Transform your Thorns stacks into Protection stacks, reducing all damage taken by {{{Buff.DamageReduction * 100}%}}",
				$"per stack for the next {{{Buff.EffectDuration}}} beats."
			),
			IconPath = "res://assets/icons/SpellBook06_09.png",
			InputType = CastInputType.Instant,
			TargetType = CastTargetType.None,
		};
		Settings.ResourceCost[ObjectResourceType.Mana] = 20;
	}

	protected override void OnCastCompleted(CastTargetData target)
	{
		var stacks = Parent.Buffs.Stacks<ShieldBash.Buff>();
		Parent.Buffs.RemoveAll<ShieldBash.Buff>();
		for (var i = 0; i < stacks; i++)
		{
			Parent.Buffs.Add(new Buff());
		}
	}

	public partial class Buff : BaseBuff
	{
		public const float DamageReduction = 0.1f;
		public const float EffectDuration = 8;

		public Buff()
		{
			Settings = new()
			{
				FriendlyName = "Shields Up",
				DynamicDesc = () => MakeDescription(
					$"Increases your damage reduction by {{{Math.Round(DamageReduction * Stacks * 100) + "%"}}}."
				),
				IconPath = "res://assets/icons/SpellBook06_09.png",
			};
			Duration = EffectDuration;
		}

		public override void ModifyUnit(BuffUnitStatsVisitor unit)
		{
			unit.PercentageDamageTaken[ObjectResourceType.Health] -= DamageReduction;
		}
	}
}