using System;
using System.Diagnostics;
using Godot;
using Project;

namespace Project;

public partial class TransitionUI : Control
{
	private ColorRect colorRect;
	private AnimationPlayer fadePlayer;

	private PackedScene transitioningTo;

	public override void _Ready()
	{
		colorRect = GetNode<ColorRect>("SceneTransitionRect");
		colorRect.Modulate = new Color(1, 1, 1, 0);
		fadePlayer = GetNode<AnimationPlayer>("SceneTransitionRect/AnimationPlayer");
		fadePlayer.AnimationFinished += OnAnimationFinished;
		SignalBus.Singleton.SceneTransitionStarted += OnSceneTransitionStarted;
		SignalBus.Singleton.SceneTransitionMusicReady += OnMusicFadedOut;
		LoadingManager.Singleton.LoadingProgressed += OnLoadingProgressed;
		LoadingManager.Singleton.LoadingCompleted += OnLoadingCompleted;
	}

	private void OnSceneTransitionStarted(PackedScene toScene)
	{
		transitioningTo = toScene;
		fadePlayer.Play("fade_out");
	}

	private void OnMusicFadedOut()
	{
		GetTree().ChangeSceneToPacked(transitioningTo);
		LoadingManager.Singleton.TriggerLoadingScreen();
	}

	private void OnLoadingProgressed(float current, float total)
	{
		SignalBus.SendMessage($"Loading assets: {Math.Round(current / total * 100)}%.");
	}

	private void OnLoadingCompleted()
	{
		FadeIn();
	}

	private async void FadeIn()
	{
		await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
		try
		{
			SignalBus.Singleton.EmitSignal(SignalBus.SignalName.SceneTransitionFinished, transitioningTo);
		}
		catch (Exception ex) { GD.PrintErr(ex); }
		fadePlayer.Play("fade_in");
	}

	private void OnAnimationFinished(StringName name)
	{
		if (name == "fade_out")
			SignalBus.Singleton.EmitSignal(SignalBus.SignalName.SceneFadeOutFinished);
		else if (name == "fade_in")
			SignalBus.Singleton.EmitSignal(SignalBus.SignalName.SceneFadeInFinished);
	}
}
