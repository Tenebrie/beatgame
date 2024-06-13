namespace Project;

public partial class SecretSkillUnlimitedPower0 : BaseSkill
{
	public SecretSkillUnlimitedPower0()
	{
		Settings = new()
		{
			FriendlyName = "Limited Power",
			IconPath = "res://assets/icons/SpellBook06_119.PNG",
			ActiveCast = CastFactory.Of<CastUnlimitedPower>(),
		};
	}
}