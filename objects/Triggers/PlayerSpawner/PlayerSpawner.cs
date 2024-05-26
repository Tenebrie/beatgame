using Godot;
using Project;
using System;
using System.Diagnostics;

namespace Project;
public partial class PlayerSpawner : Node3D
{
	private PlayerController Player;
	private PackedScene PlayerScene = GD.Load<PackedScene>("res://units/player/PlayerCharacter/PlayerCharacter.tscn");

	public override void _Ready()
	{
		SignalBus.Singleton.ResourceChanged += OnResourceChanged;
	}

	public void SpawnPlayer()
	{
		var player = PlayerScene.Instantiate<PlayerController>();
		player.Position = GlobalPosition;
		GetParent().AddChild(player);

		Player = player;
	}

	public void OnResourceChanged(BaseUnit unit, ObjectResourceType type, float value)
	{
		if (unit != Player)
			return;

		if (type == ObjectResourceType.Health && value == 0)
			Player = null;
	}

	public override void _Process(double delta)
	{
		if (Player == null)
			SpawnPlayer();
	}
}
