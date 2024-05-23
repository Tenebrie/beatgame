namespace Project;

public enum BeatTime : ulong
{
	Four = 1,
	Two = 2,
	One = 4,
	Half = 8,
	Third = 16,
	Quarter = 32,
	All = 1023,
	Free = 1024,
}

static class BeatTimeTypeExtensions
{
	public static ulong ToVariant(this BeatTime type)
	{
		return (ulong)type;
	}
}