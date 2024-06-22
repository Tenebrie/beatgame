namespace Project;

public partial class SkillShieldBashMulticast : BaseSkill
{
	public SkillShieldBashMulticast()
	{
		Settings = new()
		{
			FriendlyName = "Shield Press",
			Description = MakeDescription($"Your {{Shield Bash}} cast now deals additional {{{ShieldBash.ExtraSkillDamage}}} Spirit damage."),
			IconPath = "res://assets/icons/SpellBook06_05.png",
		};
	}
}