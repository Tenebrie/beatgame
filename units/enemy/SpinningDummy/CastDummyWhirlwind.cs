namespace Project;

public partial class CastDummyWhirlwind : BaseCast
{
	CircularTelegraph circle;

	public CastDummyWhirlwind(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			PrepareTime = 4,
			HoldTime = 12,
			AnimCast = "Whirlwind",
			InputType = CastInputType.AutoRelease,
			ChannelingTickTimings = BeatTime.EveryFullBeat,
		};
	}

	protected override void OnCastStarted(CastTargetData _)
	{
		circle = this.CreateCircularTelegraph(Parent.GetGroundedPosition());
		circle.Reparent(Parent);
		circle.Settings.GrowTime = 4;
		circle.Settings.Alliance = Parent.Alliance;
		circle.Settings.AutoCleaning = false;
		circle.Settings.Periodic = true;
		circle.Settings.Radius = 3;
		circle.Settings.TargetValidator = (unit) => unit.HostileTo(Parent);
	}

	protected override void OnCastTicked(CastTargetData _, BeatTime time)
	{
		foreach (var target in circle.GetTargets())
		{
			target.Health.Damage(10, this);
		}
	}

	protected override void OnCastCompletedOrFailed(CastTargetData _)
	{
		circle.CleanUp();
	}
}