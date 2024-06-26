namespace Project;

public partial class SkillFlagellation : BaseSkill
{
	public SkillFlagellation()
	{
		Settings = new()
		{
			FriendlyName = "Flagellation",
			IconPath = "res://assets/icons/SpellBook06_03.PNG",
			ActiveCast = CastFactory.Of<CastFlagellation>(),
		};
	}
}