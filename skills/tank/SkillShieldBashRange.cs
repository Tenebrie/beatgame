namespace Project;

public partial class SkillShieldBashRange : BaseSkill
{
	public SkillShieldBashRange()
	{
		Settings = new()
		{
			FriendlyName = "Throwing Arm",
			Description = MakeDescription($"Your {{Shield Bash}} cast range is now increased to {{{ShieldBash.IncreasedRange}}} meters."),
			IconPath = "res://assets/icons/SpellBook06_05.png",
		};
	}
}