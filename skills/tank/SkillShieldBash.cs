namespace Project;

public partial class SkillShieldBash : BaseSkill
{
	public SkillShieldBash()
	{
		Settings = new()
		{
			FriendlyName = "Shield Bash",
			IconPath = "res://assets/icons/SpellBook06_05.png",
			ActiveCast = CastFactory.Of<ShieldBash>(),
		};
	}
}