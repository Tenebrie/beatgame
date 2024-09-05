using Project;

namespace BeatGame.scripts.music;

public static class Utils
{
	public static BeatTime TickIndexToBeatTime(long tickIndex)
	{
		if (tickIndex % (int)BeatTime.Whole == 0)
			return BeatTime.Whole;
		else if (tickIndex % (int)BeatTime.Half == 0)
			return BeatTime.Half;
		else if (tickIndex % (int)BeatTime.Quarter == 0)
			return BeatTime.Quarter;
		else if (tickIndex % (int)BeatTime.Eighth == 0)
			return BeatTime.Eighth;
		else
			return BeatTime.Sixteenth;
	}
}