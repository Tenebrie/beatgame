using Godot;
using System;

namespace Project;

public partial class RespawnUI : Control
{
	[Export] Button SpawnButton;
	[Export] Button RestartButton;

	public override void _Ready()
	{
		Visible = false;
		SignalBus.Singleton.UnitKilled += OnUnitKilled;
		SpawnButton.Pressed += OnSpawn;
		RestartButton.Pressed += OnRestart;
	}

	void OnUnitKilled(BaseUnit unit)
	{
		if (unit is not PlayerController)
			return;

		Visible = true;
	}

	void OnSpawn()
	{
		foreach (var player in PlayerController.AllPlayers)
			player.QueueFree();
		Visible = false;
	}

	void OnRestart()
	{
		TimelineManager.Singleton.ResetFight();
		Visible = false;
	}
}
