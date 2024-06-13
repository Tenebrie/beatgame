namespace Project;

public partial class SkillFlamethrower : BaseSkill
{
	public SkillFlamethrower()
	{
		Settings = new()
		{
			FriendlyName = "Flamethrower",
			IconPath = "res://assets/icons/SpellBook06_117.PNG",
			ActiveCast = CastFactory.Of<CastFlamethrower>(),
		};
	}
}