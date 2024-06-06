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

	public override void _Process(double delta)
	{
		if (!IsCasting || Target == null || !IsInstanceValid(Target))
			return;

		TimeSinceLastTick += (float)delta;
		if (TimeSinceLastTick >= 0.25f)
		{
			SpawnZap();
			TimeSinceLastTick = 0;
		}
	}

	protected override void OnCastStarted(CastTargetData target)
	{
		Target = target.HostileUnit;
		TimeSinceLastTick = 0;
	}

	protected override void OnCastCompleted(CastTargetData target)
	{

	}
}
