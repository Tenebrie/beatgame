namespace Project;

public partial class BuffCastWhileMoving : BaseBuff
{
	/// <summary>Implemented internally</summary>
	public BuffCastWhileMoving()
	{
		Settings = new()
		{
			Description = "You can cast while moving."
		};
	}
}