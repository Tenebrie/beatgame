namespace Project;

public partial class SkillIgnite : BaseSkill
{
	public SkillIgnite()
	{
		Settings = new()
		{
			FriendlyName = "Ignite",
			IconPath = "res://assets/icons/SpellBook06_29.PNG",
			ActiveCast = CastFactory.Of<CastIgnite>(),
		};
	}
}