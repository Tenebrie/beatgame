namespace Project;

public enum BeatTime : ulong
{
	Whole = 1,
	Half = 2,
	Quarter = 4,
	Eighth = 8,
	Sixteenth = 16,
	EveryFullBeat = 7,
	All = 1023,
	Free = 1024,
}

static class BeatTimeTypeExtensions
{
	public static ulong ToVariant(this BeatTime type)
	{
		return (ulong)type;
	}

	public static bool Has(this BeatTime time, BeatTime comparator)
	{
		return (time & comparator) > 0;
	}

	public static bool HasNot(this BeatTime time, BeatTime comparator)
	{
		return (time & comparator) == 0;
	}
}