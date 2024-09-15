using BeatGame.scripts.navigation;
using Godot;
using Project;
using System;
using System.Linq;

namespace Project;

public partial class GiantRatBehaviour : BaseBehaviour
{
	enum AIState
	{
		Idle,
		Wandering,
		EnemySpotted,
		Attacking,
		Resting,
		Leashing,
	}

	BaseUnit lockedOnTarget;
	Vector3? movingToPosition = null;

	Node3D agentContainer;
	NavigationAgent3D navAgent;

	AIState state = AIState.Idle;
	AttackCast attackCast;

	Timer aiTickTimer;
	Timer stateLockTimer;

	Vector3 homePosition;

	public override void _Ready()
	{
		Parent.FriendlyName = "Giant Rat";

		attackCast = new(Parent);
		AddChild(attackCast);

		homePosition = Parent.GlobalPosition;

		aiTickTimer = new() { WaitTime = 0.1f };
		aiTickTimer.Timeout += OnAIUpdate;
		AddChild(aiTickTimer);
		aiTickTimer.Start();

		stateLockTimer = new() { OneShot = true };
		AddChild(stateLockTimer);

		GetComponent<AnimationController>().RegisterStateTransitions(
			("Bite", (_) => state == AIState.Attacking),
			("Walk", (_) => state is AIState.Wandering or AIState.EnemySpotted),
			("Idle", (_) => true)
		);

		navAgent = GetComponent<NavigationAgent3D>();
		navAgent.NavigationLayers = NavigationLayer.MediumAgent.ToMaskValue();
		navAgent.TargetReached += () =>
		{
			if (state is AIState.Leashing)
				state = AIState.Idle;
		};
		navAgent.VelocityComputed += (velocity) =>
		{
			if (velocity == Vector3.Zero || state is not AIState.EnemySpotted and not AIState.Leashing)
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

	void OnAIUpdate()
	{
		// Agent is locked into an action, do not update
		if (!stateLockTimer.IsStopped() || state is AIState.Leashing)
			return;

		if (Parent.GlobalPosition.DistanceTo(homePosition) >= 15)
		{
			state = AIState.Leashing;
		}
		else if (state is AIState.Idle or AIState.EnemySpotted or AIState.Attacking)
		{
			// TODO: Optimize
			var targets = BaseUnit.AllUnits
				.Where(u => u.HostileTo(Parent))
				.Select(u => (unit: u, distance: u.GlobalPosition.DistanceTo(Parent.GlobalPosition)))
				.Where(tuple => tuple.distance <= 7)
				.OrderBy(a => a.distance);

			var (unit, distance) = targets.FirstOrDefault();
			if (unit == null)
			{
				lockedOnTarget = null;
				state = AIState.Idle;

				return;
			}

			lockedOnTarget = unit;

			if (distance <= 1.5f)
			{
				stateLockTimer.Start(2f);
				state = AIState.Attacking;
			}
			else
			{
				state = AIState.EnemySpotted;
			}
		}
	}

	void PathTowards(Vector3 pos)
	{
		var target = NavigationServer3D.MapGetClosestPoint(navAgent.GetNavigationMap(), pos);
		movingToPosition = target;
		navAgent.TargetPosition = target;

		// var nextTarget = navAgent.GetNextPathPosition();
		// agentContainer.Position = new() { X = agentContainer.GlobalPosition.X, Y = nextTarget.Y, Z = agentContainer.GlobalPosition.Z };
	}

	public override void _Process(double delta)
	{
		if (state is AIState.Attacking && lockedOnTarget != null)
		{
			var castTargetData = new CastTargetData()
			{
				HostileUnit = lockedOnTarget,
			};
			if (attackCast.ValidateIfCastIsPossible(castTargetData, out _) is BaseCast.CastQueueMode.Instant)
			{
				attackCast.CastBegin(castTargetData);
			}
		}
		else if (state is AIState.EnemySpotted && lockedOnTarget != null)
		{
			PathTowards(lockedOnTarget.GlobalPosition);
		}
		else if (state is AIState.Leashing)
		{
			PathTowards(homePosition);
		}

		if (state is AIState.EnemySpotted or AIState.Leashing)
		{
			var nextPosition = navAgent.GetNextPathPosition();
			var direction = nextPosition - agentContainer.GlobalPosition;
			var velocity = 1f * direction.Normalized();
			agentContainer.GlobalPosition += velocity * (float)delta;
			nextPosition = navAgent.GetNextPathPosition();

			if (navAgent.AvoidanceEnabled)
				navAgent.Velocity = velocity;
			else
			{
				Parent.GlobalPosition = Parent.SnapToGround(agentContainer.GlobalPosition);
				if (new Vector3(velocity.X, 0, velocity.Z).LengthSquared() > 0f)
					Parent.LookAt(nextPosition.Flatten(GlobalPosition.Y), Vector3.Up);
			}
		}
	}
}

partial class AttackCast : BaseCast
{
	public AttackCast(BaseUnit parent) : base(parent)
	{
		Settings = new()
		{
			PrepareTime = 0.5f,
			RecastTime = 2,
		};
	}

	protected override void OnCastCompleted(CastTargetData targetData)
	{
		targetData.HostileUnit.Health.Damage(10f, this);
	}
}
