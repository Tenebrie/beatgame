using System;
using Godot;

namespace Project;

public partial class ControllableAudioStream : AudioStreamPlayer, IControllableEnvironment
{
	[Export]
	public string GroupName;
	[Export]
	public bool EnabledByDefault = true;
	public float EnabledVolumeDb;
	public float TargetVolumeDb;

	public override void _Ready()
	{
		EnabledVolumeDb = VolumeDb;
		if (!EnabledByDefault)
			VolumeDb = Mathf.LinearToDb(0);

		TargetVolumeDb = VolumeDb;

		if (GroupName == null)
		{
			GD.PrintErr("ControllableAudioStream is not assigned to a group");
			return;
		}
		EnvironmentController.Singleton.RegisterControllable(this, GroupName);
	}

	public void TurnOn()
	{
		TargetVolumeDb = EnabledVolumeDb;
	}

	public void TurnOff()
	{
		TargetVolumeDb = Mathf.LinearToDb(0.01f);
	}

	public override void _Process(double delta)
	{
		if (VolumeDb == TargetVolumeDb)
			return;

		VolumeDb += (TargetVolumeDb - VolumeDb) * 1f * (float)delta;
		if (Math.Abs(TargetVolumeDb - VolumeDb) <= 0.02f)
		{
			VolumeDb = TargetVolumeDb;
		}
	}
}
