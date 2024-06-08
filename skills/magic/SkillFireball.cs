namespace Project;

public partial class SkillFireball : BaseSkill
{
	public SkillFireball()
	{
		FriendlyName = "Fireball";
		IconPath = "res://assets/icons/SpellBook06_15.PNG";
		ActiveCast = CastFactory.Of<Fireball>();
	}
}