using System;
using BeatGame.scripts;
using BeatGame.scripts.music;
using Godot;

namespace Project;

public partial class PauseUI : Control
{
	[Export] Label SongNameLabel;
	[Export] Label SongTimeLabel;
	[Export] Button ContinueButton;
	[Export] Button SettingsButton;
	[Export] Button MenuButton;
	[Export] Button ExitButton;

	public override void _EnterTree()
	{
		instance = this;
		Visible = false;
	}

	public override void _Ready()
	{
		ContinueButton.Pressed += OnContinueButtonPressed;
		SettingsButton.Pressed += OnSettingsButtonPressed;
		MenuButton.Pressed += OnMenuButtonPressed;
		ExitButton.Pressed += OnExitButtonPressed;

		LoadingManager.Singleton.SceneChanged += OnSceneChanged;
	}

	private void OnContinueButtonPressed()
	{
		Visible = false;
		PauseManager.Singleton.UnpauseGame();
	}

	private void OnSettingsButtonPressed()
	{
		Visible = false;
		PauseManager.Singleton.UnpauseGame();
		SettingsUI.Singleton.Visible = true;
	}

	private void OnMenuButtonPressed()
	{
		Visible = false;
		PauseManager.Singleton.UnpauseGame();
		LoadingManager.Singleton.TransitionToScene(PlayableScene.MainMenu);
	}

	private void OnExitButtonPressed()
	{
		GetTree().Quit();
	}

	private void OnSceneChanged(PlayableScene scene)
	{
		Visible = false;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ToggleSkillForest".ToStringName()) && Visible)
		{
			Visible = false;
		}
	}

	public bool HandleEscapeKey()
	{
		Visible = !Visible;
		if (Visible)
			PauseManager.Singleton.PauseGame();
		else
			PauseManager.Singleton.UnpauseGame();
		return true;
	}

	static PauseUI instance;
	public static PauseUI Singleton => instance;
}
