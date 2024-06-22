namespace Project;

public partial class SkillAllConsumingFlame : BaseSkill
{
	public SkillAllConsumingFlame()
	{
		Settings = new()
		{
			FriendlyName = "All Consuming Flame",
			IconPath = "res://assets/icons/SpellBook06_105.png",
			ActiveCast = CastFactory.Of<CastAllConsumingFlame>(),
		};
	}
}