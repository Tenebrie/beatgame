namespace Project;

public partial class SummonCastZap : BaseCast
{
	public const float Damage = 5;
	public SummonCastZap(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Zap",
			InputType = CastInputType.Instant,
			TargetType = CastTargetType.HostileUnit,
			CastTimings = BeatTime.Whole,
		};
	}

	void SpawnZap(CastTargetData target)
	{
		var zap = Lib.LoadScene(Lib.Effect.LightningZap).Instantiate<LightningZapEffect>();
		zap.Position = Parent.GlobalCastAimPosition;
		GetTree().CurrentScene.AddChild(zap);
		zap.SetTarget(target.HostileUnit.GlobalCastAimPosition);
		zap.FadeDuration = 0.50f;

		target.HostileUnit.Health.Damage(1f, this);
	}

	protected override void OnCastCompleted(CastTargetData target)
	{
		SpawnZap(target);
	}
}
