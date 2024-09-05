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
		TrainingRoom = new MusicTrackIntermission();
		AddChild(TrainingRoom);
		BossArenaAeriel = new MusicTrackAeriel();
		AddChild(BossArenaAeriel);
		BossArenaCelestios = new MusicTrackTest();
		AddChild(BossArenaCelestios);
	}
}