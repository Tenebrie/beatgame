using System;

namespace Project;

[Flags]
public enum BeatTime : ulong
{
	Whole = 16,
	Half = 8,
	Quarter = 4,
	Eighth = 2,
	Sixteenth = 1,

	EveryFullBeat = 16 | 8 | 4,
	All = 1023,
	Free = 1024,
}

static class BeatTimeTypeExtensions
{
	public static ulong ToVariant(this BeatTime type)
	{
		return (ulong)type;
	}

	public static bool Is(this BeatTime time, BeatTime comparator)
	{
		return (time & comparator) > 0;
	}

	public static bool IsNot(this BeatTime time, BeatTime comparator)
	{
		return (time & comparator) == 0;
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