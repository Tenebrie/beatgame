namespace Project;

public partial class SkillKindness : BaseSkill
{
	public SkillKindness()
	{
		Settings = new()
		{
			FriendlyName = "Kindness",
			Description = MakeDescription(
				$"Your {{{"Healing Touch"}}} skill can now target allies as well as yourself."
			),
			IconPath = "res://assets/icons/SpellBook06_55.png",
		};
	}
}