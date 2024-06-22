namespace Project;

public partial class SkillVampiricEssence : BaseSkill
{
	public SkillVampiricEssence()
	{
		Settings = new()
		{
			FriendlyName = "Vampiric Essence",
			IconPath = "res://assets/icons/SpellBook06_105.png",
			PassiveBuff = BuffFactory.Of<BuffMagicLifeLeechPassive>(),
		};
	}
}