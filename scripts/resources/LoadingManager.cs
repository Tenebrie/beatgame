using System;
using System.Collections.Generic;
using Godot;

namespace Project;

public partial class LoadingManager : Node
{
	[Signal]
	public delegate void LoadingProgressedEventHandler(float current, float total);
	[Signal]
	public delegate void LoadingCompletedEventHandler();
	public override void _Ready()
	{
		instance = this;
	}

	public async void TriggerLoadingScreen()
	{
		// int resourcesProcessed = 0;
		// var resources = AssetManager.GetAllSceneResources();
		// List<Node3D> things = new();
		// foreach (var res in resources)
		// {
		// 	try
		// 	{
		// 		var instance = GD.Load<PackedScene>(res).Instantiate<Node3D>();
		// 		instance.Position = new Vector3(0, -10, 0);
		// 		instance.SetProcess(false);
		// 		GetTree().CurrentScene.AddChild(instance);
		// 		things.Add(instance);
		// 	}
		// 	catch (Exception) { }
		// 	resourcesProcessed += 1;
		// 	EmitSignal(SignalName.LoadingProgressed, resourcesProcessed, resources.Count * 2);
		// }
		// await ToSignal(GetTree().CreateTimer(0), "timeout");

		// foreach (var res in things)
		// {
		// 	res.QueueFree();
		// 	resourcesProcessed += 1;
		// 	EmitSignal(SignalName.LoadingProgressed, resourcesProcessed, resources.Count * 2);
		// }

		await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
		EmitSignal(SignalName.LoadingCompleted);
	}

	private static LoadingManager instance = null;
	public static LoadingManager Singleton
	{
		get => instance;
	}
}