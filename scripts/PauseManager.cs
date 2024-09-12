using Godot;
using Project;

namespace BeatGame.scripts;

public partial class PauseManager : Node
{
	public override void _EnterTree()
	{
		instance = this;
	}

	public void PauseGame()
	{
		GetTree().Paused = true;
		CastUtils.NotifyAboutPause();
	}

	public void UnpauseGame()
	{
		GetTree().Paused = false;
		CastUtils.NotifyAboutUnpause();
	}

	static PauseManager instance = null;
	public static PauseManager Singleton => instance;
}