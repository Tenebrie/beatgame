namespace Project;

public partial class BuffIgnite : BaseBuff
{
	public const float DamagePerBeat = 5;
	public const float BurnDuration = 16;

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

	public override void OnBeatTick(BeatTime time, int stacks)
	{
		if (time.HasNot(BeatTime.EveryFullBeat))
			return;

		Parent.Health.Damage(DamagePerBeat * stacks, SourceCast);
	}
}
