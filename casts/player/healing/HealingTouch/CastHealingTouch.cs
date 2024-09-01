using Godot;

namespace Project;

public partial class CastHealingTouch : BaseCast
{
	float HealAmount = 150;
	public CastHealingTouch(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Healing Touch",
			Description = $"Look into yourself for reserves of inner strength. Restores [color={Colors.Health}]{HealAmount}[/color] Health to yourself.",
			IconPath = "res://assets/icons/SpellBook06_55.png",
			InputType = CastInputType.AutoRelease,
			TargetType = CastTargetType.None,
			HoldTime = 1,
		};
		Settings.ResourceCost[ObjectResourceType.Mana] = 40;

		if (this.HasSkill<SkillKindness>())
			Settings.TargetType = CastTargetType.AlliedUnitOrSelf;
	}

	protected override void OnCastCompleted(CastTargetData data)
	{
		BaseUnit target = Parent;
		if (this.HasSkill<SkillKindness>())
			target = data.AlliedUnit;

		var healImpact = Lib.LoadScene(Lib.Effect.HealImpact).Instantiate() as ProjectileImpact;
		healImpact.AttachForDuration(target, .3f, target.CastAimPosition);

		var healing = Flags.CastSuccessful ? HealAmount : HealAmount / 2;
		target.Health.Restore(healing, this);
	}
}