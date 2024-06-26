namespace Project;

public enum DamageType : int
{
	Physical = 1,
	Fire = 2,
	Night = 4,
	All = 1023,
}

public enum SilentDamageReason : int
{
	Retaliate,
}

static class SilentDamageTypeExtensions
{
	public static int ToVariant(this SilentDamageReason type)
	{
		return (int)type;
	}
}