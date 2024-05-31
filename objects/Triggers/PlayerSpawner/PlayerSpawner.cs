using Godot;
using Project;
using System;
using System.Diagnostics;

namespace Project;
public partial class PlayerSpawner : Node3D
{
	private PlayerController Player;

	public override void _Ready()
	{
		SignalBus.Singleton.UnitDestroyed += OnUnitDestroyed;
	}

	public void SpawnPlayer()
	{
		var player = Lib.Scene(Lib.Unit.PlayerCharacter).Instantiate<PlayerController>();
		player.Position = GlobalPosition;
		GetParent().AddChild(player);

		Player = player;
	}

	public void OnUnitDestroyed(BaseUnit unit)
	{
		if (unit != Player)
			return;

		Player = null;
	}

	public override void _Process(double delta)
	{
		if (Player == null)
			SpawnPlayer();
	}
}
