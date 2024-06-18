namespace Project;

public partial class BuffMinorIgnite : BaseBuff
{
	public const float DamagePerBeat = 5;
	public const float BurnDuration = 8;
	
	public BuffMinorIgnite()
	{
		Settings = new()
		{
			Description = $"Deals {Colors.Tag(DamagePerBeat)} Fire damage per beat.",
			TicksOnBeat = true,
		};
		Duration = BurnDuration;
	}

	protected override void OnBeatTick(BeatTime time)
	{
		if (time.HasNot(BeatTime.Quarter))
			return;

		Parent.Health.Damage(DamagePerBeat, SourceCast);
	}
}