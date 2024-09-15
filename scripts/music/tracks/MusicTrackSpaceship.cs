using Project;

namespace BeatGame.scripts.music;

public partial class MusicTrackSpaceship : MusicTrack
{
	public MusicTrackSpaceship()
	{
		FullName = "Mersona - Spaceship";
		BeatsPerMinute = 120;
		ResourcePath = Lib.Audio.MusicTrackSpaceship;
	}
}