using Godot;
using Project;

public partial class PauseUI : Control
{
	private Button ExitButton;
	private HSlider VolumeSlider;
	private HSlider CameraHeightSlider;

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
		CameraHeightSlider = GetNode<HSlider>("Panel/CameraHeightSlider");
		CameraHeightSlider.Value = Preferences.Singleton.CameraHeight * 100;
		CameraHeightSlider.ValueChanged += (double value) => Preferences.Singleton.CameraHeight = (float)value / 100;
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
