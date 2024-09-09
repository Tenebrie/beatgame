using Godot;

namespace Project;

public partial class MainMenuUI : Control
{
	[Export] Button ContinueButton;
	[Export] Button NewGameButton;
	[Export] Button LoadGameButton;
	[Export] Button SettingsButton;
	[Export] Button ExitButton;

	public override void _EnterTree()
	{
		ContinueButton.Pressed += OnNewGame;
		NewGameButton.Pressed += OnNewGame;
		LoadGameButton.Pressed += OnNewGame;
		SettingsButton.Pressed += OnSettings;
		ExitButton.Pressed += () => GetTree().Quit();
		LoadingManager.Singleton.SceneChanged += OnSceneChanged;
	}

	void OnNewGame()
	{
		LoadingManager.Singleton.TransitionToScene(PlayableScene.TrainingRoom);
	}

	void OnSettings()
	{
		SignalBus.SendMessage("Settings is not implemented yet :(");
	}

	private void OnSceneChanged(PlayableScene scene)
	{
		Visible = scene == PlayableScene.MainMenu;
	}
}