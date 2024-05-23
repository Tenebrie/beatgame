using Godot;
using Project;
using System;
using System.Diagnostics;

public partial class PauseUI : Control
{
	private Button ExitButton;
	private HSlider VolumeSlider;

	public override void _EnterTree()
	{
		Visible = false;
	}
	public override void _Ready()
	{
		ExitButton = GetNode<Button>("Panel/ExitGameButton");
		ExitButton.Pressed += OnExitButtonPressed;
		VolumeSlider = GetNode<HSlider>("Panel/VolumeSlider");
		VolumeSlider.Value = Preferences.Singleton.MainVolume * 100;
		VolumeSlider.ValueChanged += OnVolumeChanged;
	}

	private void OnExitButtonPressed()
	{
		GetTree().Quit();
	}

	private void OnVolumeChanged(double value)
	{
		Preferences.Singleton.MainVolume = (float)value / 100;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("Escape"))
		{
			Visible = !Visible;
		}
	}
}
