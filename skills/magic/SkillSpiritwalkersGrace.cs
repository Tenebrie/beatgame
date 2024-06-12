namespace Project;

public partial class SkillSpiritwalkersGrace : BaseSkill
{
	public SkillSpiritwalkersGrace()
	{
		Settings = new()
		{
			FriendlyName = "Spiritwalker's Grace",
			IconPath = "res://assets/icons/SpellBook06_13.png",
			ActiveCast = CastFactory.Of<CastSpiritwalkersGrace>(),
		};
	}
}