namespace Project;

public partial class SkillQuickDash : BaseSkill
{
	public SkillQuickDash()
	{
		Settings = new()
		{
			FriendlyName = "Quick Dash",
			IconPath = "res://assets/icons/SpellBook06_21.PNG",
			ActiveCast = CastFactory.Of<CastQuickDash>(),
		};
	}
}