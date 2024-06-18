namespace Project;

public partial class SkillEtherealDarkness : BaseSkill
{
	public const float HealthPerCast = 30;
	public SkillEtherealDarkness()
	{
		Settings = new()
		{
			FriendlyName = "Ethereal Darkness",
			Description = MakeDescription(
				$"Your {{{"Ethereal Focus"}}} cast is now instant and restores the full amount of Mana.",
				$"\nHowever, it now requires you to spend {{{HealthPerCast}}} Health to cast."
			),
			IconPath = "res://assets/icons/SpellBook06_46.PNG",
		};
	}
}