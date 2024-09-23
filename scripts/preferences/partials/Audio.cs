using Godot;

namespace BeatGame.scripts.preferences;

public partial class Preferences : Node
{
	public void ApplyAudioVolume()
	{
		AudioServer.SetBusVolumeDb(0, Mathf.LinearToDb(MainVolume));
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Music"), Mathf.LinearToDb(MusicVolume));
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Effects"), Mathf.LinearToDb(AudioVolume));
	}
}