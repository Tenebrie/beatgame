namespace Project;

public partial class SkillSentinelMana : BaseSkill
{
	public SkillSentinelMana()
	{
		Settings = new()
		{
			FriendlyName = "Mana Sentinel",
			Description = MakeDescription(
				$"Your {Colors.Tag("Sentinel")} cast does no longer have a cooldown, but instead costs {Colors.Tag(CastSentinel.ManaCost, Colors.Mana)} Mana."
			),
			IconPath = "res://assets/icons/SpellBook06_78.png",
			AffectedCasts = new() { CastFactory.Of<CastSentinel>() },
			IncompatibleSkills = new() { SkillWrapper.Of<SkillSentinelCharges>() }
		};
	}
}