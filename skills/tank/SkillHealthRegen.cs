namespace Project;

public partial class SkillHealthRegen : BaseSkill
{
	public SkillHealthRegen()
	{
		Settings = new()
		{
			FriendlyName = "Regeneration",
			IconPath = "res://assets/icons/SpellBook06_07.png",
			PassiveBuff = BuffFactory.Of<BuffHealthRegen>()
		};
	}
}

public partial class SkillHealthRegen1 : SkillHealthRegen { }
public partial class SkillHealthRegen2 : SkillHealthRegen { }
public partial class SkillHealthRegen3 : SkillHealthRegen { }