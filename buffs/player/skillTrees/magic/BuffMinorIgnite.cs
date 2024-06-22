namespace Project;

public partial class BuffMinorIgnite : BaseBuff
{
	public const float DamagePerBeat = 2;
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

	public override void OnBeatTick(BeatTime time, int stacks)
	{
		if (time.HasNot(BeatTime.EveryFullBeat))
			return;

		Parent.Health.Damage(DamagePerBeat * stacks, SourceCast);
	}
}