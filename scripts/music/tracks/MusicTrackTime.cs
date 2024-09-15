using Project;

namespace BeatGame.scripts.music;

public partial class MusicTrackTime : MusicTrack
{
	public MusicTrackTime()
	{
		FullName = "Mersona - Time";
		BeatsPerMinute = 80;
		ResourcePath = Lib.Audio.MusicTrackTime;
		Loop = true;
	}
}