using Project;

namespace Project;

public partial class VirtualCastRetaliation : BaseCast
{
	public VirtualCastRetaliation(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			FriendlyName = "Retaliation",
			Description = $"Whenever you take damage, deal a fraction of it back to the attacker.",
			IconPath = "res://assets/icons/SpellBook06_66.png",
		};
	}
}