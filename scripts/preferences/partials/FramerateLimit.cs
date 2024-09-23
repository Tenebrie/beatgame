using Godot;

namespace BeatGame.scripts.preferences;

public partial class Preferences : Node
{
	public void ApplyFramerateLimit()
	{
		Engine.Singleton.MaxFps = FpsLimit <= 240 ? FpsLimit : 1000;
	}
}