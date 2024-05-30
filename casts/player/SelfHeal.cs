using Godot;

namespace Project;

public partial class SelfHeal : BaseCast
{
	public SelfHeal(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			InputType = CastInputType.HoldRelease,
			TargetType = CastTargetType.None,
			CastTimings = BeatTime.One,
		};
	}

	protected override void OnCastCompleted(CastTargetData _)
	{
		var healImpact = Lib.Scene(Lib.Effect.HealImpact).Instantiate() as ProjectileImpact;
		healImpact.AttachForDuration(Parent, .3f, new Vector3(0, 0.25f, 0));

		var healing = Flags.CastSuccessful ? 60 : 30;
		Parent.Health.Restore(healing);
		Parent.Mana.Damage(10);
	}
}