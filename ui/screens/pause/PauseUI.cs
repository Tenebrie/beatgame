using System;
using BeatGame.scripts.music;
using Godot;

namespace Project;

public partial class PauseUI : Control
{
	[Export] HSlider MainVolumeSlider;
	[Export] HSlider MusicVolumeSlider;
	[Export] HSlider EffectVolumeSlider;
	[Export] HSlider CameraHeightSlider;
	[Export] HSlider MsaaSlider;
	[Export] Button MenuButton;
	[Export] Button ExitButton;

	public override void _EnterTree()
	{
		Visible = false;
	}

	public override void _Ready()
	{
		MainVolumeSlider.Value = Preferences.Singleton.MainVolume * 100;
		MainVolumeSlider.ValueChanged += (double value) => Preferences.Singleton.MainVolume = (float)value / 100;

		MusicVolumeSlider.Value = Preferences.Singleton.MusicVolume * 100;
		MusicVolumeSlider.ValueChanged += (double value) => Preferences.Singleton.MusicVolume = (float)value / 100;

		EffectVolumeSlider.Value = Preferences.Singleton.AudioVolume * 100;
		EffectVolumeSlider.ValueChanged += (double value) => Preferences.Singleton.AudioVolume = (float)value / 100;

		CameraHeightSlider.Value = Preferences.Singleton.CameraHeight * 100;
		CameraHeightSlider.ValueChanged += (double value) => Preferences.Singleton.CameraHeight = (float)value / 100;

		MsaaSlider.Value = Preferences.Singleton.MsaaLevel;
		MsaaSlider.ValueChanged += (double value) => Preferences.Singleton.MsaaLevel = (int)value;

		MenuButton.Pressed += OnMenuButtonPressed;
		ExitButton.Pressed += OnExitButtonPressed;

		LoadingManager.Singleton.SceneChanged += OnSceneChanged;
	}

	private void OnMenuButtonPressed()
	{
		Visible = false;
		UnpauseGame();
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

	public override void _UnhandledInput(InputEvent @event)
	{
		if (SceneManager.Singleton.CurrentScene == PlayableScene.MainMenu)
			return;

		if (@event.IsActionPressed("Escape".ToStringName()))
		{
			Visible = !Visible;
			if (Visible)
				PauseGame();
			else
				UnpauseGame();
		}
	}

	void PauseGame()
	{
		GetTree().Paused = true;
		CastUtils.NotifyAboutPause();
	}

	void UnpauseGame()
	{
		GetTree().Paused = false;
		CastUtils.NotifyAboutUnpause();
	}
}
