using Godot;

namespace Project;

public partial class ControllableDirectionalLight : DirectionalLight3D, IControllableLight
{
	[Export]
	public string GroupName;
	[Export]
	public bool EnabledByDefault = true;
	public float EnabledEnergy;

	public override void _Ready()
	{
		EnabledEnergy = LightEnergy;
		if (!EnabledByDefault)
			LightEnergy = 0;

		if (GroupName == null)
		{
			GD.PrintErr("ControllableLight is not assigned to a group");
			return;
		}
		EnvironmentController.Singleton.RegisterControllableLight(this, GroupName);
	}

	public void TurnOn()
	{
		LightEnergy = EnabledEnergy;
	}

	public void TurnOff()
	{
		LightEnergy = 0;
	}
}