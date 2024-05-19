namespace Project;

public enum ObjectResourceType : int
{
	Health,
	Mana,
}

static class ObjectResourceTypeExtensions
{
	public static int ToVariant(this ObjectResourceType type)
	{
		return (int)type;
	}
}