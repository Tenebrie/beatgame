namespace Project;

public partial class SkillVaporize : BaseSkill
{
	public SkillVaporize()
	{
		Settings = new()
		{
			FriendlyName = "Vaporize",
			IconPath = "res://assets/icons/SpellBook06_23.PNG",
			ActiveCast = CastFactory.Of<CastVaporize>(),
		};
	}
}