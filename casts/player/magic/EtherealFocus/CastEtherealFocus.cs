using Godot;

namespace Project;
public partial class CastEtherealFocus : BaseCast
{
	public const float ManaPerBeat = 10;
	public const float ManaBurst = 50;

	BaseEffect channelEffect;

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
			InputType = CastInputType.HoldRelease,
			CastTimings = BeatTime.Whole | BeatTime.Half,
			ChannelingTickTimings = BeatTime.All,
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
		channelEffect = this.CreateEffect(Lib.Effect.EtherealFocusChannel, Parent.GlobalCastAimPosition).Attach(Parent);
	}

	protected override void OnCastTicked(CastTargetData _, BeatTime time)
	{
		Parent.Mana.Restore(ManaPerBeat * Music.MinBeatSize, this);
	}

	protected override void OnCastCompleted(CastTargetData target)
	{
		Parent.Mana.Restore(ManaBurst, this);
		this.CreateEffect(Lib.Effect.EtherealFocusBurst, Parent.GlobalCastAimPosition).SetLifetime(1);
	}

	protected override void OnCastCompletedOrFailed(CastTargetData target)
	{
		channelEffect.Stop().FreeAfterDelay();
	}
}