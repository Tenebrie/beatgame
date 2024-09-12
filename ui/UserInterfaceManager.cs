using Godot;

namespace Project;

public partial class UserInterfaceManager : Node
{
	static PauseUI Pause => PauseUI.Singleton;
	static SettingsUI Settings => SettingsUI.Singleton;
	static SkillForestUI SkillForest => SkillForestUI.Singleton;

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("Escape".ToStringName()))
		{
			HandleEscapeKey();
		}
	}

	static void HandleEscapeKey()
	{
		if (SkillForest.Visible && SkillForest.HandleEscapeKey())
			return;

		if (Settings.Visible && Settings.DirtyModal.Visible && Settings.DirtyModal.HandleEscapeKey())
			return;

		if (Settings.Visible && Settings.HandleEscapeKey())
		{
			return;
		}

		if (SceneManager.Singleton.CurrentScene == PlayableScene.MainMenu)
			return;

		Pause.HandleEscapeKey();
	}
}