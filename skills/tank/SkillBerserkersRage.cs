using Godot;

namespace Project;

public partial class SkillBerserkersRage : BaseSkill
{
	public SkillBerserkersRage()
	{
		Settings = new()
		{
			FriendlyName = "Berserker's Rage",
			IconPath = "res://assets/icons/SpellBook06_65.png",
			PassiveBuff = BuffFactory.Of<SkillBuff>(),
		};
	}

	public partial class SkillBuff : BaseBuff
	{
		public float LastTriggerAt = 0;
		public const float TriggerCooldown = 1;
		public SkillBuff()
		{
			Settings = new()
			{
				Description = MakeDescription(
					$"Whenever you take damage, gain {{Rage}}, increasing all your damage dealt by {{{RageBuff.DamageBoost * 100}%}}",
					$"for the next {{{RageBuff.EffectDuration}}} beats, stacking up to {{{RageBuff.MaximumStacks}}}.",
					$"\n((This effect can only trigger once per beat))"
				),
				Hidden = true,
			};
		}

		public override void ReactToIncomingDamage(BuffIncomingDamageVisitor damage)
		{
			if (damage.ResourceType != ObjectResourceType.Health)
				return;

			var time = ((float)Time.GetTicksMsec()) / 1000;
			if (time - LastTriggerAt < Music.Singleton.SecondsPerBeat - AccurateTimer.TimingWindow)
				return;

			LastTriggerAt = time;
			this.NextFrame(() => Parent.Buffs.Add(new RageBuff()));
		}

		public partial class RageBuff : BaseBuff
		{
			public const float DamageBoost = 0.01f;
			public const float EffectDuration = 16;
			public const int MaximumStacks = 300;

			public RageBuff()
			{
				Settings = new()
				{
					FriendlyName = "Rage",
					Description = MakeDescription(
						$"All your damage is increased by {{{DamageBoost * 100}%}}."
					),
					IconPath = "res://assets/icons/SpellBook06_65.png",
					RefreshOthersWhenAdded = true,
					MaximumStacks = MaximumStacks,
				};
				Duration = EffectDuration;
			}

			public override void ModifyOutgoingDamage(BuffOutgoingDamageVisitor damage)
			{
				if (damage.Target == Parent || damage.ResourceType != ObjectResourceType.Health)
					return;

				damage.Value += damage.BaseValue * DamageBoost;
			}
		}
	}
}