namespace Project;

public partial class BuffIgnite : BaseBuff
{
	public const float DamagePerBeat = 5;
	public const float BurnDuration = 32;

	public BuffIgnite()
	{
		Settings = new()
		{
			FriendlyName = "Ignite",
			Description = $"Deals {Colors.Tag(DamagePerBeat)} Fire damage per beat.",
			IconPath = "res://assets/icons/SpellBook06_29.PNG",
			TicksOnBeat = true,
			MaximumStacks = 1,
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
