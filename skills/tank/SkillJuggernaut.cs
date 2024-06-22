namespace Project;

public partial class SkillJuggernaut : BaseSkill
{
	public SkillJuggernaut()
	{
		Settings = new()
		{
			FriendlyName = "Juggernaut",
			IconPath = "res://assets/icons/SpellBook06_112.png",
			ActiveCast = CastFactory.Of<CastJuggernaut>(),
		};
	}
}