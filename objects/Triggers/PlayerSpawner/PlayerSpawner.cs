using Godot;
using Project;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Project;
public partial class PlayerSpawner : Node3D
{
	[Export]
	public Mode SpawnMode = Mode.Always;
	Node3D Visuals;
	private PlayerController Player;
	private static Dictionary<string, BaseCast> SavedCastBindings;

	public override void _Ready()
	{
		Visuals = GetNode<Node3D>("Visuals");
		Visuals.Hide();
		SignalBus.Singleton.UnitCreated += OnUnitCreated;
		SignalBus.Singleton.UnitDestroyed += OnUnitDestroyed;
	}

	public void SpawnPlayer()
	{
		var player = Lib.Scene(Lib.Unit.PlayerCharacter).Instantiate<PlayerController>();
		player.Position = GlobalPosition;
		GetTree().CurrentScene.AddChild(player);

		if (SavedCastBindings != null)
			player.Spellcasting.LoadBindings(SavedCastBindings);
		SavedCastBindings = null;

		Player = player;
	}

	public void OnUnitCreated(BaseUnit unit)
	{
		if (unit is not PlayerController player)
			return;

		Player = player;
	}

	public void OnUnitDestroyed(BaseUnit unit)
	{
		if (unit != Player || unit is not PlayerController player)
			return;

		SavedCastBindings = player.Spellcasting.CastBindings;
		Player = null;
	}

	public override void _Process(double delta)
	{
		var isInCombat = Music.Singleton.IsStarted;
		var shouldSpawnInCombat = SpawnMode == Mode.InCombat;
		if (Player == null && (SpawnMode == Mode.Always || isInCombat == shouldSpawnInCombat))
			SpawnPlayer();
	}

	public enum Mode
	{
		InCombat,
		OutOfCombat,
		Always,
	}
}
