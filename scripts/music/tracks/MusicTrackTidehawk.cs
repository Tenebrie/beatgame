using Project;

namespace BeatGame.scripts.music;

public partial class MusicTrackTidehawk : MusicTrack
{
	public MusicTrackTidehawk()
	{
		FullName = "Mersona - Tidehawk";
		BeatsPerMinute = 120;
		ResourcePath = Lib.Audio.MusicTrackTidehawk;
	}
}