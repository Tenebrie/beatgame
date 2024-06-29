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
		LoadingManager.Singleton.SceneTransitioned += OnSceneTransitioned;
	}

	void OnNewGame()
	{
		LoadingManager.Singleton.TransitionToScene(PlayableScene.TrainingRoom);
	}

	void OnSettings()
	{
		
	}

	private void OnSceneTransitioned(PlayableScene scene)
	{
		Visible = scene == PlayableScene.MainMenu;
	}
}