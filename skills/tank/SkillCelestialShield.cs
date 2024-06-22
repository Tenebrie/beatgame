namespace Project;

public partial class SkillCelestialShield : BaseSkill
{
	public SkillCelestialShield()
	{
		Settings = new()
		{
			FriendlyName = "Celestial Shield",
			IconPath = "res://assets/icons/SpellBook06_12.png",
			ActiveCast = CastFactory.Of<CastCelestialShield>(),
		};
	}
}