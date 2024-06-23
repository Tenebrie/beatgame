namespace Project;

public partial class BuffCastWhileMoving : BaseBuff
{
	/// <summary>Implemented internally</summary>
	public BuffCastWhileMoving()
	{
		Settings = new()
		{
			FriendlyName = "Swift Feet",
			Description = "You can cast while moving.",
			IconPath = "res://assets/icons/SpellBook06_13.png",
		};
	}
}