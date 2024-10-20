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

		LoadingManager.Singleton.TransitionToScene(TransitionTo);
	}
}
