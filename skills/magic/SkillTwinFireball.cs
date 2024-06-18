namespace Project;

public partial class SkillTwinFireball : BaseSkill
{
	public static float ExtraManaCost = 10f;
	public SkillTwinFireball()
	{
		Settings = new()
		{
			FriendlyName = "Twin Fireball",
			Description = MakeDescription(
				$"Your {Colors.Tag("Fireball")} now creates an extra projectile which targets the same enemy.",
				$"\nIncreases the {Colors.Tag("Fireball")} mana cost by {Colors.Tag(ExtraManaCost, Colors.Mana)}."
			),
			IconPath = "res://assets/icons/SpellBook06_15.PNG",
		};
	}
}