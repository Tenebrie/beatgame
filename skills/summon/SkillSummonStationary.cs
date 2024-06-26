namespace Project;

public partial class SkillSummonStationary : BaseSkill
{
	public SkillSummonStationary()
	{
		Settings = new()
		{
			FriendlyName = "Summon Totem",
			IconPath = "res://assets/icons/SpellBook06_44.PNG",
			ActiveCast = CastFactory.Of<CastSummonStationary>(),
			PassiveBuff = BuffFactory.Of<BuffSummonTreeSummonHealth>(descriptionOnly: true)
		};
	}
}