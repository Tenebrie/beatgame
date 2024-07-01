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
		LoadingManager.Singleton.StateChanged += OnLoadingStateChanged;
	}

	void OnLoadingStateChanged(LoadingManager.State state)
	{
		if (state == LoadingManager.State.FadeOutStarted)
			FadeOut();
		if (state == LoadingManager.State.FadeInStarted)
			FadeIn();
	}

	private void FadeOut()
	{
		fadePlayer.Play("fade_out".ToStringName());
	}

	private void FadeIn()
	{
		fadePlayer.Play("fade_in".ToStringName());
	}

	private void OnAnimationFinished(StringName name)
	{
		if (name == "fade_out".ToStringName())
			SignalBus.Singleton.EmitSignal(SignalBus.SignalName.SceneFadeOutFinished);
		else if (name == "fade_in".ToStringName())
			SignalBus.Singleton.EmitSignal(SignalBus.SignalName.SceneFadeInFinished);
	}
}
