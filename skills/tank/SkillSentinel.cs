namespace Project;

public partial class SkillSentinel : BaseSkill
{
	public SkillSentinel()
	{
		Settings = new()
		{
			FriendlyName = "Sentinel",
			IconPath = "res://assets/icons/SpellBook06_78.png",
			ActiveCast = CastFactory.Of<CastSentinel>(),
			PassiveBuff = BuffFactory.Of<BuffTankTreeHealth>(descriptionOnly: true)
		};
	}
}