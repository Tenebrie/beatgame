using System;
using System.Collections.Generic;
using Godot;

namespace Project;

public partial class LoadingManager : Node
{
	[Signal] public delegate void SceneTransitionStartedEventHandler(PlayableScene scene);
	[Signal] public delegate void SceneTransitionedEventHandler(PlayableScene scene);
	[Signal] public delegate void SceneTransitionFinishedEventHandler(PlayableScene scene);
	[Signal] public delegate void StateChangedEventHandler(State state);
	[Signal] public delegate void ResourceLoadingProgressedEventHandler(float current, float total);

	public PlayableScene TransitioningTo;
	public State CurrentState = State.Loaded;
	public HashSet<FadeOutComponent> ReadyComponents = new();

	public override void _EnterTree()
	{
		instance = this;

		// Call a fade in on first game load

		// Components must declare they are done with fade out before continuing
		SignalBus.Singleton.SceneFadeOutFinished += () => MarkComponentAsReady(FadeOutComponent.TransitionUI);
		SignalBus.Singleton.SceneTransitionMusicReady += () => MarkComponentAsReady(FadeOutComponent.Music);

		// When fade in is finished, declare scene to be loaded
		SignalBus.Singleton.SceneFadeInFinished += () =>
		{
			if (CurrentState == State.Loaded)
				return;

			SetState(State.Loaded);
			EmitSignal(SignalName.SceneTransitionFinished, TransitioningTo.ToVariant());
		};
	}

	public override void _Ready()
	{
		var path = GetTree().CurrentScene.SceneFilePath;
		var playableScene = Lib.Scene.ToEnum(path);
		EmitSignal(SignalName.SceneTransitioned, playableScene.ToVariant());
	}

	public void TransitionToCurrentScene()
	{
		TransitionToScene(Lib.LoadScene(GetTree().CurrentScene.SceneFilePath));
	}

	public void TransitionToScene(PlayableScene scene)
	{
		var packedScene = Lib.LoadScene(Lib.Scene.ToPath(scene));
		TransitionToScene(packedScene);
	}

	public void TransitionToScene(PackedScene scene)
	{
		var playableScene = Lib.Scene.ToEnum(scene.ResourcePath);
		TransitioningTo = playableScene;
		SetState(State.FadeOutStarted);
		ReadyComponents.Clear();
		EmitSignal(SignalName.SceneTransitionStarted, playableScene.ToVariant());
	}

	void MarkComponentAsReady(FadeOutComponent component)
	{
		if (CurrentState != State.FadeOutStarted)
			return;

		ReadyComponents.Add(component);
		if (CurrentState == State.FadeOutStarted && ReadyComponents.Count == Enum.GetNames(typeof(FadeOutComponent)).Length)
		{
			SetState(State.ComponentsReady);
			TriggerLoadingScreen();
		}
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

		var targetScene = GD.Load<PackedScene>(Lib.Scene.ToPath(TransitioningTo));
		GetTree().ChangeSceneToPacked(targetScene);
		EmitSignal(SignalName.SceneTransitioned, TransitioningTo.ToVariant());
		await ToSignal(GetTree().CreateTimer(0.5f), "timeout".ToStringName());
		SetState(State.FadeInStarted);
	}

	void SetState(State state)
	{
		CurrentState = state;
		EmitSignal(SignalName.StateChanged, (int)state);
	}

	private static LoadingManager instance = null;
	public static LoadingManager Singleton
	{
		get => instance;
	}

	public enum State : int
	{
		FadeOutStarted,
		ComponentsReady,
		FadeInStarted,
		Loaded,
	}

	public enum FadeOutComponent
	{
		Music,
		TransitionUI,
	}
}