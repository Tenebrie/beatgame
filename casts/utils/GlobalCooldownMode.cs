namespace Project;

public enum GlobalCooldownMode
{
	Ignore = 0,
	Trigger = 1,
	Receive = 2,
	Normal = Trigger | Receive,
}

public static class GlobalCooldownModeExtensions
{
	public static bool Triggers(this GlobalCooldownMode mode)
	{
		return (mode & GlobalCooldownMode.Trigger) > 0;
	}

	public static bool Receives(this GlobalCooldownMode mode)
	{
		return (mode & GlobalCooldownMode.Receive) > 0;
	}
}