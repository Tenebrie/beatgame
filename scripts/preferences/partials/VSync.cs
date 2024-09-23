using Godot;

namespace BeatGame.scripts.preferences;

public partial class Preferences : Node
{
	public void ApplyVSync()
	{
		DisplayServer.Singleton.WindowSetVsyncMode(VSyncMode);
	}
}