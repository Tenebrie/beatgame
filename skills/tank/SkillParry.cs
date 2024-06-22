namespace Project;

public partial class SkillParry : BaseSkill
{
	public SkillParry()
	{
		Settings = new()
		{
			FriendlyName = "Parry",
			IconPath = "res://assets/icons/SpellBook06_123.png",
			ActiveCast = CastFactory.Of<CastParry>(),
		};
	}
}