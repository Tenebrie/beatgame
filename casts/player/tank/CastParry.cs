using System;
using Godot;

namespace Project;

public partial class CastParry : BaseCast
{
	BuffParry buff;

	public CastParry(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Parry",
			Description = MakeDescription(
				$"For the next {{{1}}} beat, block all incoming damage.",
				$"When you finish the cast, release {{{BuffParry.RetaliateDamageFraction * 100 + "%"}}} of it back at the enemy."
			),
			LoreDescription = "With a lightning-fast strike, you block the enemy attack and riposte at the same moment.",
			IconPath = "res://assets/icons/SpellBook06_123.png",
			InputType = CastInputType.HoldRelease,
			TargetType = CastTargetType.None,
			CastTimings = BeatTime.EveryFullBeat,
			HoldTime = 1,
			RecastTime = 8,
			GlobalCooldown = true,
		};
		Settings.ResourceCost[ObjectResourceType.Mana] = 50;
	}

	protected override void OnCastStarted(CastTargetData _)
	{
		buff = new BuffParry()
		{
			SourceCast = this,
		};
		Parent.Buffs.Add(buff);
	}

	protected override void OnCastCompleted(CastTargetData _)
	{
		var storedDamage = buff.StoredDamage;
		foreach (var entry in storedDamage)
		{
			var zap = this.CreateZapEffect(Parent.GlobalCastAimPosition, entry.Key.GlobalCastAimPosition);
			zap.AnimationFinished += () => entry.Key.Health.Damage(entry.Value, this);
			this.CreateZapEffect(Parent.GlobalCastAimPosition, entry.Key.GlobalCastAimPosition);
			this.CreateZapEffect(Parent.GlobalCastAimPosition, entry.Key.GlobalCastAimPosition);
		}
		Parent.Buffs.Remove(buff);
	}

	protected override void OnCastFailed(CastTargetData _)
	{
		Parent.Buffs.Remove(buff);
		Parent.Buffs.Recalculate();
		var storedDamage = buff.StoredDamage;
		foreach (var entry in storedDamage)
			Parent.Health.Damage(entry.Value, entry.Key);
	}
}