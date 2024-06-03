using System;
using Godot;

namespace Project;

public partial class ControllableDirectionalLight : DirectionalLight3D, IControllableLight
{
	[Export]
	public string GroupName;
	[Export]
	public bool EnabledByDefault = true;
	public float EnabledEnergy;
	public float TargetEnergy;

	public override void _Ready()
	{
		EnabledEnergy = LightEnergy;
		if (!EnabledByDefault)
			LightEnergy = 0;

		TargetEnergy = LightEnergy;

		if (GroupName == null)
		{
			GD.PrintErr("ControllableLight is not assigned to a group");
			return;
		}
		EnvironmentController.Singleton.RegisterControllableLight(this, GroupName);
	}

	public void TurnOn()
	{
		TargetEnergy = EnabledEnergy;
	}

	public void TurnOff()
	{
		TargetEnergy = 0;
	}

	public override void _Process(double delta)
	{
		if (LightEnergy == TargetEnergy)
			return;

		LightEnergy += (TargetEnergy - LightEnergy) * 5f * (float)delta;
		if (Math.Abs(TargetEnergy - LightEnergy) <= 0.05f)
		{
			LightEnergy = TargetEnergy;
		}
	}
}