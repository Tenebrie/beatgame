namespace Project;

public partial class SkillIgnitingFireball : BaseSkill
{
	public static float ExtraManaCost = 10f;
	public SkillIgnitingFireball()
	{
		Settings = new()
		{
			FriendlyName = "Igniting Fireball",
			Description = MakeDescription(
				$"Your {Colors.Tag("Fireball")} now applies a {Colors.Tag("Minor Ignite")}, dealing",
				$"{Colors.Tag(BuffMinorIgnite.DamagePerBeat)} damage per beat for {BuffMinorIgnite.BurnDuration} beats.",
				$"\nIncreases the {Colors.Tag("Fireball")} mana cost by {Colors.Tag(ExtraManaCost, Colors.Mana)}."
			),
			IconPath = "res://assets/icons/SpellBook06_15.PNG",
		};
	}
}