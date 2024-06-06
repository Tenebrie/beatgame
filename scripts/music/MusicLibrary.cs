using Godot;

namespace Project;
public partial class MusicLibrary : Node
{
	public MusicTrack TrainingRoom;
	public MusicTrack BossArenaAeriel;

	public override void _EnterTree()
	{
		TrainingRoom = new MusicTrackIntermission();
		AddChild(TrainingRoom);
		BossArenaAeriel = new MusicTrackAeriel();
		AddChild(BossArenaAeriel);
	}
}