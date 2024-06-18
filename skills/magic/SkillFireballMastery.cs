namespace Project;

public partial class SkillFireballMastery : BaseSkill
{
	public SkillFireballMastery()
	{
		Settings = new()
		{
			FriendlyName = "Fireball Mastery",
			Description = MakeDescription(
				$"Your {Colors.Tag("Fireball")} is now cast instantly and doesn't interrupt other spells.",
				$"All the other effects of the spell are unchanged."
			),
			IconPath = "res://assets/icons/SpellBook06_15.PNG",
		};
	}
}