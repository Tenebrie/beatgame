namespace Project;

public partial class BuffMinorIgnite : BaseBuff
{
	public const float DamagePerBeat = 2;
	public const float BurnDuration = 12;

	public BuffMinorIgnite()
	{
		Settings = new()
		{
			FriendlyName = "Minor Ignite",
			DynamicDesc = () => $"Deals {Colors.Tag(DamagePerBeat * Stacks)} Fire damage per beat.",
			IconPath = "res://assets/icons/SpellBook06_29.PNG",
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