using Godot;

namespace Project;
public partial class CastFlamethrower : BaseCast
{
	float DamagePerBeat = 40;
	EffectFlamethrowerWithHitbox effect;

	public CastFlamethrower(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Flamethrower",
			Description = MakeDescription(
				$"Create an inferno of flames dealing {Colors.Tag(DamagePerBeat)} Fire Damage per beat to all units in a cone."
			),
			LoreDescription = MakeDescription(
				$"You are not so easily fooled by so-called war crimes. Any weapon that is effective is a legitimate strategy.",
				"A spell that is spewing hell at the enemy is no exception."
			),
			IconPath = "res://assets/icons/SpellBook06_117.PNG",
			InputType = CastInputType.AutoRelease,
			TargetType = CastTargetType.HostileUnit,
			CastTimings = BeatTime.Whole | BeatTime.Half,
			ChannelingTickTimings = BeatTime.All,
			HoldTime = 8,
			RecastTime = 16
		};
		Settings.ResourceCost[ObjectResourceType.Mana] = 10;
		Settings.ResourceCostPerBeat[ObjectResourceType.Mana] = 10;
	}

	protected override void OnCastStarted(CastTargetData target)
	{
		effect = Lib.LoadScene(Lib.Token.EffectFlamethrowerWithHitbox).Instantiate<EffectFlamethrowerWithHitbox>();
		effect.TargetUnit = target.HostileUnit;
		effect.DamagePerBeat = DamagePerBeat;
		effect.FollowedUnit = Parent;
		effect.SourceCast = this;
		effect.TargetValidator = (unit) => unit.HostileTo(Parent);
		effect.Position = Parent.CastAimPosition;
		Parent.AddChild(effect);
	}

	protected override void OnCastTicked(CastTargetData target, BeatTime time)
	{
		if (!IsInstanceValid(target.HostileUnit))
			CastComplete();
	}

	protected override void OnCastCompletedOrFailed(CastTargetData _)
	{
		effect.CleanUp();
	}
}