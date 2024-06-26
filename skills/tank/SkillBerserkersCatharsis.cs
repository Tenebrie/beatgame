namespace Project;

public partial class SkillBerserkersCatharsis : BaseSkill
{
	public SkillBerserkersCatharsis()
	{
		Settings = new()
		{
			FriendlyName = "Berserker's Catharsis",
			IconPath = "res://assets/icons/SpellBook06_65.png",
			ActiveCast = CastFactory.Of<BerserkersCatharsis>(),
		};
	}
}