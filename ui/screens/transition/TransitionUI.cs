using System.Diagnostics;
using Godot;
using Project;

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
		SignalBus.Singleton.SceneTransitionMusicReady += OnMusicReady;

	}

	private void OnSceneTransitionStarted(PackedScene toScene)
	{
		transitioningTo = toScene;
		fadePlayer.Play("fade_out");
	}

	private async void OnMusicReady()
	{
		GetTree().ChangeSceneToPacked(transitioningTo);
		await ToSignal(GetTree().CreateTimer(0), "timeout");
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.SceneTransitionFinished, transitioningTo);
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
