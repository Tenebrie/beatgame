namespace Project;

public partial class SkillShieldsUp : BaseSkill
{
	public SkillShieldsUp()
	{
		Settings = new()
		{
			FriendlyName = "Shields Up",
			IconPath = "res://assets/icons/SpellBook06_09.png",
			ActiveCast = CastFactory.Of<ShieldsUp>(),
		};
	}
}