using Godot;

namespace Project;

public partial class PauseUI : Control
{
	private HSlider VolumeSlider;
	private HSlider CameraHeightSlider;
	private Button ChillModeButton;
	private Button ExitButton;

	public override void _EnterTree()
	{
		Visible = false;
	}
	public override void _Ready()
	{
		VolumeSlider = GetNode<HSlider>("Panel/VBoxContainer/Volume/Slider");
		VolumeSlider.Value = Preferences.Singleton.MainVolume * 100;
		VolumeSlider.ValueChanged += OnVolumeChanged;
		CameraHeightSlider = GetNode<HSlider>("Panel/VBoxContainer/CameraHeight/Slider");
		CameraHeightSlider.Value = Preferences.Singleton.CameraHeight * 100;
		CameraHeightSlider.ValueChanged += (double value) => Preferences.Singleton.CameraHeight = (float)value / 100;
		ChillModeButton = GetNode<Button>("Panel/VBoxContainer/ChillMode/CheckButton");
		ChillModeButton.SetPressedNoSignal(Preferences.Singleton.ChillMode);
		ChillModeButton.Toggled += (bool value) => Preferences.Singleton.ChillMode = value;
		ExitButton = GetNode<Button>("Panel/ExitGameButton");
		ExitButton.Pressed += OnExitButtonPressed;
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
