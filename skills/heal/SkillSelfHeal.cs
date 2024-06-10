namespace Project;

public partial class SkillSelfHeal : BaseSkill
{
	public SkillSelfHeal()
	{
		Settings = new()
		{
			FriendlyName = "Introspection",
			IconPath = "res://assets/icons/SpellBook06_55.png",
			ActiveCast = CastFactory.Of<SelfHeal>(),
		};
	}
}