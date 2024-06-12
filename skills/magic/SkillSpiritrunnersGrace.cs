namespace Project;

public partial class SkillSpiritrunnersGrace : BaseSkill
{
	public SkillSpiritrunnersGrace()
	{
		Settings = new()
		{
			FriendlyName = "Spiritrunner's Grace",
			Description = "While Spiritwalker's Grace is active, you move at full speed.",
			IconPath = "res://assets/icons/SpellBook06_13.png",
			AffectedCasts = new () { CastFactory.Of<CastSpiritwalkersGrace>() },
		};
	}
}