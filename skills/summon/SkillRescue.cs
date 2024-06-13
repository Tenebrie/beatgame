namespace Project;

public partial class SkillRescue : BaseSkill
{
	public SkillRescue()
	{
		Settings = new()
		{
			FriendlyName = "Rescue",
			IconPath = "res://assets/icons/SpellBook06_101.png",
			ActiveCast = CastFactory.Of<CastRescue>(),
		};
	}
}