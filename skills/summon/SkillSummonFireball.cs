namespace Project;

public partial class SkillSummonFireball : BaseSkill
{
	public SkillSummonFireball()
	{
		Settings = new()
		{
			FriendlyName = "Allied Fireball",
			Description = MakeDescription(
				$"Add a Fireball spell to your ally's repertoire. This spell gains all the bonuses your Fireball does.\n\n",
				$"{Colors.Tag("Fireball:", Colors.Active)}",
				CastFactory.Of<Fireball>().Settings.Description
			),
			IconPath = "res://assets/icons/SpellBook06_15.PNG",
		};
	}
}