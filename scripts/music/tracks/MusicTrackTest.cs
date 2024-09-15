using Project;

namespace BeatGame.scripts.music;

public partial class MusicTrackTest : MusicTrack
{
	public MusicTrackTest()
	{
		FullName = "Mersona - ???";
		BeatsPerMinute = 120;
		ResourcePath = Lib.Audio.MusicTrackTest;
		Loop = true;
	}
}