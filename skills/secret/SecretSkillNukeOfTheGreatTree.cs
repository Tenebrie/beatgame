namespace Project;

public partial class SecretSkillNukeOfTheGreatTree : BaseSkill
{
	public SecretSkillNukeOfTheGreatTree()
	{
		Settings = new()
		{
			FriendlyName = "Nuke of the Great Tree",
			IconPath = "res://assets/icons/SpellBook06_57.png",
			ActiveCast = CastFactory.Of<NukeOfTheGreatTree>(),
		};
	}
}