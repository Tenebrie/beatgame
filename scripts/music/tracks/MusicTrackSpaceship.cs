using Project;

namespace BeatGame.scripts.music;
public partial class MusicTrackSpaceship : MusicTrack
{
	public MusicTrackSpaceship()
	{
		BeatsPerMinute = 120;
		ResourcePath = Lib.Audio.MusicTrackSpaceship;
	}
}