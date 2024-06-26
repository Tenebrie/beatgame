using Godot;

namespace Project;
public partial class SceneTransition : Area3D
{
	[Export]
	private PlayableScene TransitionTo;

	public override void _Ready()
	{
		BodyEntered += OnAreaEntered;
	}

	private void OnAreaEntered(Node3D body)
	{
		if (body is not PlayerController)
			return;

		SignalBus.SendMessage("The boss is not yet available, come back later!", 5);

		// var transitionToScene = Lib.LoadScene(Lib.Scene.GetPath(TransitionTo));
		// SignalBus.Singleton.EmitSignal(SignalBus.SignalName.SceneTransitionStarted, transitionToScene);
	}
}
