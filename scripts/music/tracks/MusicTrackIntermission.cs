using Project;

namespace BeatGame.scripts.music;
public partial class MusicTrackIntermission : MusicTrack
{
	public MusicTrackIntermission()
	{
		BeatsPerMinute = 70;
		ResourcePath = Lib.Audio.MusicTrackIntermission;
		Loop = true;
	}
}