using Godot;

namespace Project;

public partial class PauseUI : Control
{
	[Export] HSlider MainVolumeSlider;
	[Export] HSlider MusicVolumeSlider;
	[Export] HSlider CameraHeightSlider;
	[Export] Button ChillModeButton;
	[Export] Button ExitButton;

	public override void _EnterTree()
	{
		Visible = false;
	}
	public override void _Ready()
	{
		MainVolumeSlider.Value = Preferences.Singleton.MainVolume * 100;
		MainVolumeSlider.ValueChanged += OnMainVolumeChanged;
		MusicVolumeSlider.Value = Preferences.Singleton.MusicVolume * 100;
		MusicVolumeSlider.ValueChanged += OnMusicVolumeChanged;
		CameraHeightSlider.Value = Preferences.Singleton.CameraHeight * 100;
		CameraHeightSlider.ValueChanged += (double value) => Preferences.Singleton.CameraHeight = (float)value / 100;
		ChillModeButton.SetPressedNoSignal(Preferences.Singleton.ChillMode);
		ChillModeButton.Toggled += (bool value) => Preferences.Singleton.ChillMode = value;
		ExitButton.Pressed += OnExitButtonPressed;
	}

	private void OnExitButtonPressed()
	{
		GetTree().Quit();
	}

	private void OnMainVolumeChanged(double value)
	{
		Preferences.Singleton.MainVolume = (float)value / 100;
	}

	private void OnMusicVolumeChanged(double value)
	{
		Preferences.Singleton.MusicVolume = (float)value / 100;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ToggleSkillForest") && Visible)
		{
			Visible = false;
		}
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("Escape"))
		{
			Visible = !Visible;
		}
	}
}
