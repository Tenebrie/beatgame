namespace Project;

public partial class BuffIgnite : BaseBuff
{
	public const float DamagePerBeat = 10;
	public const float BurnDuration = 32;

	public BuffIgnite()
	{
		Settings = new()
		{
			Description = $"Deals {Colors.Tag(DamagePerBeat)} Fire damage per beat.",
			TicksOnBeat = true,
			PreventStacking = true,
		};
		Duration = BurnDuration;
	}

	protected override void OnBeatTick(BeatTime time)
	{
		if (time.HasNot(BeatTime.EveryFullBeat))
			return;

		Parent.Health.Damage(DamagePerBeat, SourceCast);
	}
}
