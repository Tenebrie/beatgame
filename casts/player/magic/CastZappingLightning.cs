namespace Project;

public partial class CastZappingLightning : BaseCast
{
	BaseUnit Target;
	float TimeSinceLastTick = 0;
	public CastZappingLightning(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Zapping Lightning",
			InputType = CastInputType.AutoRelease,
			TargetType = CastTargetType.HostileUnit,
			CastTimings = BeatTime.Whole | BeatTime.Half | BeatTime.Quarter,
			ChannelingTickTimings = BeatTime.Whole | BeatTime.Half | BeatTime.Quarter,
			HoldTime = 8,
		};
	}

	void SpawnZap()
	{
		var zap = Lib.LoadScene(Lib.Effect.LightningZap).Instantiate<LightningZapEffect>();
		zap.Position = Parent.GlobalCastAimPosition;
		GetTree().CurrentScene.AddChild(zap);
		zap.SetTarget(Target.GlobalCastAimPosition);
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
