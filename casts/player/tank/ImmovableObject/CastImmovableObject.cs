using System;
using Godot;

namespace Project;

public partial class CastImmovableObject : BaseCast
{
	public CastImmovableObject(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Immovable Object",
			Description = MakeDescription(
				$"Brace yourself firmly on the ground, ignoring any forceful movement or crowd control effects for the next",
				$"{{{Buff.EffectDuration}}} beats.",
				"However, you also lose the ability to move yourself for the duration."
			),
			IconPath = "res://assets/icons/SpellBook06_74.png",
			InputType = CastInputType.Instant,
			TargetType = CastTargetType.None,
			CastTimings = BeatTime.Free,
			RecastTime = 16,
			Charges = 1,
			GlobalCooldown = false,
		};
	}

	protected override void OnCastCompleted(CastTargetData _)
	{
		Parent.Buffs.Add(new Buff()
		{
			SourceCast = this,
		});
	}

	public partial class Buff : BaseBuff
	{
		public const float EffectPower = 1.0f;
		public const float EffectDuration = 4;

		public Buff()
		{
			Settings = new()
			{
				Description = MakeDescription(
					$"Increases your crowd control resistance by {{{Math.Round(EffectPower * 100) + "%"}}}."
				),
				IconPath = "res://assets/icons/SpellBook06_74.png",
			};
			Duration = EffectDuration;
			// if (!SourceCast.HasSkill<EquivalentMobility>())
			// {
			Settings.Description += $"\nAlso reduces your movement speed to {{0}}.";
			// }
		}

		public override void ModifyUnit(BuffUnitStatsVisitor unit)
		{
			unit.PercentageCCReduction += EffectPower;
			// If 
			// if (!SourceCast.HasSkill<EquivalentMobility>())
			// {
			unit.MoveSpeedPercentage = 0;
			// }
		}
	}
}