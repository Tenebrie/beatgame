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

	void OnMusicUpdate()
	{
		if (!Visible)
			return;

		var track = Music.Singleton.CurrentTrack;
		if (track is null)
			return;

		SongNameLabel.Text = $"{track.FullName} ({track.BeatsPerMinute} BPM)";
		SongTimeLabel.Text = $"{SecondsToTimeString(track.CurrentTime)} / {SecondsToTimeString(track.Length)}";
	}

	static string SecondsToTimeString(float seconds)
	{
		var minutes = (int)(seconds / 60);
		var remainingSeconds = (int)(seconds % 60);
		return $"{minutes:00}:{remainingSeconds:00}";
	}

	public bool HandleEscapeKey()
	{
		Visible = !Visible;
		if (Visible)
		{
			PauseManager.Singleton.PauseGame();
			OnMusicUpdate();
		}
		else
			PauseManager.Singleton.UnpauseGame();
		return true;
	}

	static PauseUI instance;
	public static PauseUI Singleton => instance;
}
