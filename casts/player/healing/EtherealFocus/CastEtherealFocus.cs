using Godot;

namespace Project;
public partial class CastEtherealFocus : BaseCast
{
	public const float ManaPerBeat = 10;
	public const float ManaBurst = 50;

	SimpleParticleEffect channelEffect;

	public CastEtherealFocus(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Ethereal Focus",
			Description = MakeDescription(
				$"Channel a reserve of your power, restoring {{{ManaPerBeat}}} Mana per beat.",
				$"\nIf you sustain the spell for the entire duration, gain an extra {{{ManaBurst}}} Mana instantly."
			),
			IconPath = "res://assets/icons/SpellBook06_46.PNG",
			InputType = CastInputType.AutoRelease,
			TickDuration = 0.125f,
			HoldTime = 4,
			RecastTime = 2,
		};

		if (this.HasSkill<SkillEtherealDarkness>())
		{
			Settings.HoldTime = 0;
			Settings.InputType = CastInputType.Instant;
			Settings.ResourceCost[ObjectResourceType.Health] += SkillEtherealDarkness.HealthPerCast;
		}
	}

	protected override void OnCastStarted(CastTargetData _)
	{
		channelEffect = this.CreateSimpleParticleEffect(Lib.Effect.EtherealFocusChannel, Parent.GlobalCastAimPosition);
		channelEffect.Attach(Parent);
	}

	protected override void OnCastTicked(CastTargetData _)
	{
		Parent.Mana.Restore(ManaPerBeat * Settings.TickDuration, this);
	}

	protected override void OnCastCompleted(CastTargetData target)
	{
		Parent.Mana.Restore(ManaBurst, this);
		this.CreateSimpleParticleEffect(Lib.Effect.EtherealFocusBurst, Parent.GlobalCastAimPosition).SetLifetime(1);
	}

	protected override void OnCastCompletedOrFailed(CastTargetData target)
	{
		channelEffect.Stop().FreeAfterDelay();
	}
}