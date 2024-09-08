using Godot;
using Project;

namespace BeatGame.scripts.music;

public partial class MusicLibrary : Node
{
	public MusicTrack TrainingRoom;
	public MusicTrack BossArenaAeriel;
	public MusicTrack BossArenaCelestios;

	public override void _EnterTree()
	{
		instance = this;

		TrainingRoom = new MusicTrackIntermission();
		AddChild(TrainingRoom);
		BossArenaAeriel = new MusicTrackAeriel();
		AddChild(BossArenaAeriel);
		BossArenaCelestios = new MusicTrackTest();
		AddChild(BossArenaCelestios);
	}

	private static MusicLibrary instance = null;
	public static MusicLibrary Singleton => instance;
}