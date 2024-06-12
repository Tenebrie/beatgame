namespace Project;

public partial class SkillSentinelCharges : BaseSkill
{
	public SkillSentinelCharges()
	{
		Settings = new()
		{
			FriendlyName = "Twice the Sentinel",
			Description = MakeDescription(
				$"Your {Colors.Tag("Sentinel")} can now store an additional charge."
			),
			IconPath = "res://assets/icons/SpellBook06_78.png",
			AffectedCasts = new() { CastFactory.Of<CastSentinel>() },
			IncompatibleSkills = new() { SkillWrapper.Of<SkillSentinelMana>() }
		};
	}
}