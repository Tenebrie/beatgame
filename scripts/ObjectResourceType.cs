namespace Project;

public enum ObjectResourceType
{
	Health,
}

static class ObjectResourceTypeExtensions
{
	public static int ToVariant(this ObjectResourceType type)
	{
		return (int)type;
	}
}