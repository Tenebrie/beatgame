namespace Project;

public partial class CastZap : BaseCast
{
	BaseUnit Target;
	float TimeSinceLastTick = 0;
	public CastZap(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Zap",
			InputType = CastInputType.AutoRelease,
			TargetType = CastTargetType.HostileUnit,
			CastTimings = BeatTime.Whole | BeatTime.Half | BeatTime.Quarter,
			ChannelingTickTimings = BeatTime.Whole | BeatTime.Half | BeatTime.Quarter,
			HoldTime = 8,
			// RecastTime = 0.5f,
		};
	}

	void SpawnZap()
	{
		var zap = Lib.Scene(Lib.Effect.LightningZap).Instantiate<LightningZapEffect>();
		zap.Position = Parent.CastAimPosition;
		GetTree().CurrentScene.AddChild(zap);
		zap.SetTarget(Target.CastAimPosition);
		zap.FadeDuration = 0.50f;

		Target.Health.Damage(1f, this);
	}

	protected override void OnCastTicked(CastTargetData target, BeatTime time)
	{
		if (Target == null || !IsInstanceValid(Target))
			return;

		SpawnZap();
	}

	protected override void OnCastStarted(CastTargetData target)
	{
		Target = target.HostileUnit;
		TimeSinceLastTick = 0;

		SpawnZap();
	}

	protected override void OnCastCompleted(CastTargetData target)
	{

	}
}
