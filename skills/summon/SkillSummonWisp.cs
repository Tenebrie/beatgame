namespace Project;

public partial class SkillSummonWisp : BaseSkill
{
	public SkillSummonWisp()
	{
		Settings = new()
		{
			FriendlyName = "Summon Wisp",
			IconPath = "res://assets/icons/SpellBook06_58.png",
			ActiveCast = CastFactory.Of<CastSummonWisp>(),
		};
	}
}