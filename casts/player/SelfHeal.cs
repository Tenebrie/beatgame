using Godot;

namespace Project;

public partial class SelfHeal : BaseCast
{
	float HealAmount = 60;
	public SelfHeal(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Self Heal",
			Description = $"Look into yourself for reserves of inner strength. Restores [color={Colors.Health}]{HealAmount}[/color] Health.",
			IconPath = "res://assets/icons/SpellBook06_55.png",
			InputType = CastInputType.HoldRelease,
			TargetType = CastTargetType.None,
			HoldTime = 1,
			CastTimings = BeatTime.Whole | BeatTime.Half | BeatTime.Quarter,
		};
		Settings.ResourceCost[ObjectResourceType.Mana] = 20;
	}

	protected override void OnCastCompleted(CastTargetData _)
	{
		var healImpact = Lib.Scene(Lib.Effect.HealImpact).Instantiate() as ProjectileImpact;
		healImpact.AttachForDuration(Parent, .3f, new Vector3(0, 0.25f, 0));

		var healing = Flags.CastSuccessful ? HealAmount : HealAmount / 2;
		Parent.Health.Restore(healing, this);
	}
}