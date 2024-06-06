namespace Project;
public partial class MusicTrackTest : MusicTrack
{
	public MusicTrackTest()
	{
		BeatsPerMinute = 120;
		ResourcePath = Lib.Audio.MusicTrackTest;
		Loop = true;
	}
}