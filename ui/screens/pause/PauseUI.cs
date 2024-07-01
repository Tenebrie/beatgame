using System;
using Godot;

namespace Project;

public partial class PauseUI : Control
{
	[Export] HSlider MainVolumeSlider;
	[Export] HSlider MusicVolumeSlider;
	[Export] HSlider CameraHeightSlider;
	[Export] Button ChillModeButton;
	[Export] HSlider MsaaSlider;
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

		CameraHeightSlider.Value = Preferences.Singleton.CameraHeight * 100;
		CameraHeightSlider.ValueChanged += (double value) => Preferences.Singleton.CameraHeight = (float)value / 100;

		ChillModeButton.SetPressedNoSignal(Preferences.Singleton.ChillMode);
		ChillModeButton.Toggled += (bool value) => Preferences.Singleton.ChillMode = value;

		MsaaSlider.Value = Preferences.Singleton.MsaaLevel;
		MsaaSlider.ValueChanged += (double value) => Preferences.Singleton.MsaaLevel = (int)value;

		ExitButton.Pressed += OnExitButtonPressed;
	}

	private void OnExitButtonPressed()
	{
		GetTree().Quit();
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
		if (@event.IsActionPressed("Escape".ToStringName()))
		{
			Visible = !Visible;
		}
	}
}
