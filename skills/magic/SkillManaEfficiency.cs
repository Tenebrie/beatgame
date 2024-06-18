namespace Project;

public partial class SkillManaEfficiency : BaseSkill
{
	public SkillManaEfficiency()
	{
		Settings = new()
		{
			FriendlyName = "Mana Efficiency",
			IconPath = "res://assets/icons/SpellBook06_52.PNG",
			PassiveBuff = BuffFactory.Of<BuffPlus10ManaEfficiency>(),
			RebindsAllCasts = true,
		};
	}
}