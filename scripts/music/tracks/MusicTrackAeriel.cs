using Project;

namespace BeatGame.scripts.music;
public partial class MusicTrackAeriel : MusicTrack
{
	public MusicTrackAeriel()
	{
		BeatsPerMinute = 120;
		ResourcePath = Lib.Audio.MusicTrackAeriel;
	}
}