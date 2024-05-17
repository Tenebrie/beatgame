using Godot;

namespace Project;

public partial class SelfHeal : BaseCast
{
	public SelfHeal(BaseUnit parent) : base(parent)
	{
		InputType = CastInputType.HoldRelease;
		TargetType = CastTargetType.None;
		CastTimings = BeatTime.One;
	}

	protected override void CastOnNone()
	{
		var scene = GD.Load<PackedScene>("res://effects/HealImpact/HealImpact.tscn");
		var healImpact = scene.Instantiate() as ProjectileImpact;
		healImpact.AttachForDuration(Parent, .3f, new Vector3(0, 0.25f, 0));

		Parent.Health.Restore(10);
	}
}