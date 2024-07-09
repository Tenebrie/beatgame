using Godot;

namespace Project;

public partial class PlayerAnimation : ComposableScript
{
	AnimationTree animationTree;
	AnimationNodeBlendTree blendTree;
	AnimationNodeStateMachinePlayback movementState;
	AnimationNodeStateMachinePlayback rotationState;

	new readonly PlayerController Parent;

	public PlayerAnimation(BaseUnit parent) : base(parent)
	{
		Parent = parent as PlayerController;
	}

	public override void _Ready()
	{
		animationTree = Parent.GetNode<AnimationTree>("MainAnimationTree");
		blendTree = (AnimationNodeBlendTree)animationTree.TreeRoot;
		movementState = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/MovementStateMachine/playback");
		rotationState = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/RotationStateMachine/playback");
	}

	public override void _Process(double delta)
	{
		var isWalkingForward = Parent.Movement.InputVector.Y > 0;
		var isWalkingBackwards = Parent.Movement.InputVector.Y < 0;

		if (isWalkingForward)
			movementState.Travel("WalkCycle");
		else if (isWalkingBackwards)
			movementState.Travel("WalkCycleBack");
		else
			movementState.Travel("Idle");

		var isRotatingLeft = Parent.Movement.RotationInput > 0;
		var isRotatingRight = Parent.Movement.RotationInput < 0;
		if (isRotatingLeft)
			rotationState.Travel("RotateLeft");
		else if (isRotatingRight)
			rotationState.Travel("RotateRight");
		else
			rotationState.Travel("Idle");
	}

	public Vector3 GetRootMotionPosition()
	{
		return animationTree.GetRootMotionPosition();
	}

	public Quaternion GetRootMotionRotation()
	{
		return animationTree.GetRootMotionRotation();
	}
}