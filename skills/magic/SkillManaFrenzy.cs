namespace Project;

public partial class SkillManaFrenzy : BaseSkill
{
	public SkillManaFrenzy()
	{
		Settings = new()
		{
			FriendlyName = "Mana Frenzy",
			Description = MakeDescription(
				$"Whenever you cast {Colors.Tag("Fireball")}, {Colors.Tag("Ignite")}, {Colors.Tag("Flamethrower")} or {Colors.Tag("Vaporize")},",
				$"you get a stack of {Colors.Tag("Mana Frenzy")}, increasing your damage dealt by {Colors.Tag("1%")}.",
				$"\nStacks disappear if you haven't casted a spell in the past {Colors.Tag(BuffManaFrenzy.FrenzyDuration)} beats."
			),
			IconPath = "res://assets/icons/SpellBook06_53.PNG",
		};
	}
}