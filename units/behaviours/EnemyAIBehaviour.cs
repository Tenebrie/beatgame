using BeatGame.scripts.navigation;
using Godot;
using System;

namespace Project;

public enum AIState
{
	Idle,
	Wandering,
	EnemySpotted,
	Attacking,
	Resting,
	Leashing,
}

[GlobalClass]
public partial class EnemyAIBehaviour : BaseBehaviour
{
	[Export] NavigationLayer agentSize = NavigationLayer.MediumAgent;

	[Signal] public delegate void UpdateTickEventHandler();

	// Can be externally controlled
	public AIState State = AIState.Idle;
	// Can be externally controlled
	public BaseUnit TargetEnemy;
	// Can be externally controlled
	public Vector3 HomePosition;
	// Can be externally controlled
	public float AgentMaxSpeed;

	Timer aiTickTimer;
	Timer stateLockTimer;

	Node3D agentContainer;
	NavigationAgent3D navAgent;

	public override void _Ready()
	{
		HomePosition = Parent.GlobalPosition;

		aiTickTimer = new() { WaitTime = 0.1f };
		aiTickTimer.Timeout += () =>
		{
			if (IsBusy())
				return;

			if (Parent.GlobalPosition.DistanceTo(HomePosition) >= 30)
			{
				State = AIState.Leashing;
				return;
			}

			if (TargetEnemy != null && (!IsInstanceValid(TargetEnemy) || TargetEnemy.IsDead || TargetEnemy.GlobalPosition.DistanceTo(Parent.GlobalPosition) > 20))
			{
				TargetEnemy = null;
				State = AIState.Idle;
				return;
			}

			EmitSignal(SignalName.UpdateTick);
		};

		AddChild(aiTickTimer);
		aiTickTimer.Start();

		stateLockTimer = new() { OneShot = true };
		AddChild(stateLockTimer);

		navAgent = GetComponent<NavigationAgent3D>();
		navAgent.NavigationLayers = agentSize.ToMaskValue();
		navAgent.TargetReached += () =>
		{
			if (State is AIState.Leashing)
				State = AIState.Idle;
		};

		navAgent.VelocityComputed += (velocity) =>
		{
			if (velocity == Vector3.Zero || State is not AIState.EnemySpotted and not AIState.Leashing)
				return;
			// TODO: Make it move
		};

		this.CallDeferred(() =>
		{
			navAgent.GetParent().RemoveChild(navAgent);
			agentContainer = new Node3D();
			GetTree().CurrentScene.AddChild(agentContainer);
			agentContainer.AddChild(navAgent);
			agentContainer.GlobalPosition = Parent.GlobalPosition;
		});
	}

	public override void _PhysicsProcess(double delta)
	{
		if (State is AIState.Leashing)
		{
			PathTowards(HomePosition);
		}

		if (State is AIState.EnemySpotted or AIState.Leashing)
		{
			var nextPosition = navAgent.GetNextPathPosition();
			var direction = nextPosition - agentContainer.GlobalPosition;
			var velocity = AgentMaxSpeed * direction.Normalized();
			agentContainer.GlobalPosition += velocity * (float)delta;
			nextPosition = navAgent.GetNextPathPosition();

			if (navAgent.AvoidanceEnabled)
				navAgent.Velocity = velocity;
			else
			{
				Parent.GlobalPosition = Parent.SnapToGround(agentContainer.GlobalPosition);
				try
				{
					if (new Vector3(velocity.X, 0, velocity.Z).LengthSquared() > 0f)
						Parent.LookAt(nextPosition.Flatten(GlobalPosition.Y), Vector3.Up);
				}
				catch (Exception) { }
			}
		}
	}

	public void PathTowards(Vector3 pos)
	{
		var target = NavigationServer3D.MapGetClosestPoint(navAgent.GetNavigationMap(), pos);
		navAgent.TargetPosition = target;

		// var nextTarget = navAgent.GetNextPathPosition();
		// agentContainer.Position = new() { X = agentContainer.GlobalPosition.X, Y = nextTarget.Y, Z = agentContainer.GlobalPosition.Z };
	}

	public float PathDistanceTo(Vector3 pos)
	{
		var a = new NavigationPathQueryParameters3D
		{
			Map = navAgent.GetNavigationMap(),
			StartPosition = Parent.GlobalPosition,
			TargetPosition = pos
		};
		var b = new NavigationPathQueryResult3D();
		NavigationServer3D.QueryPath(a, b);
		var totalDist = 0f;
		var lastPoint = Parent.GlobalPosition;
		for (var i = 0; i < b.Path.Length - 1; i++)
		{
			totalDist += lastPoint.DistanceTo(b.Path[i]);
			lastPoint = b.Path[i];
		}
		return totalDist;
	}

	public void MakeBusy(float duration)
	{
		stateLockTimer.Start(duration);
	}

	public bool IsBusy()
	{
		return !stateLockTimer.IsStopped() || State is AIState.Leashing or AIState.Resting;
	}
}
