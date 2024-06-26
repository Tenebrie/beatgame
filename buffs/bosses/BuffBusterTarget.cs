namespace Project;

public partial class BuffBusterTarget : BaseBuff
{
	public BuffBusterTarget(BaseCast source)
	{
		Settings = new()
		{
			FriendlyName = "Tank Buster Target",
			Description = MakeDescription("You are about to receive a lot of damage."),
			IconPath = "res://assets/icons/SpellBook06_41.PNG",
		};
	}
}