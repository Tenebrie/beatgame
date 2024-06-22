namespace Project;

public partial class SkillImmovableObject : BaseSkill
{
	public SkillImmovableObject()
	{
		Settings = new()
		{
			FriendlyName = "Immovable Object",
			IconPath = "res://assets/icons/SpellBook06_74.png",
			ActiveCast = CastFactory.Of<CastImmovableObject>(),
		};
	}
}