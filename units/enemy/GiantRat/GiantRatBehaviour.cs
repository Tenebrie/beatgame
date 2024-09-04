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
		GetComponent<NavigationAgent3D>().TargetReached += () =>
		{
			if (state is AIState.Leashing)
				state = AIState.Idle;
		};
		GetComponent<NavigationAgent3D>().VelocityComputed += (velocity) =>
		{
			if (velocity == Vector3.Zero || state is not AIState.EnemySpotted and not AIState.Leashing)
				return;
			Parent.Velocity = velocity;
			Parent.MoveAndSlide();
			Parent.LookAt((GlobalPosition + velocity).Flatten(GlobalPosition.Y), Vector3.Up);
		};
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
		var target = CastUtils.SnapToGround(Parent, pos);
		movingToPosition = target;
		GetComponent<NavigationAgent3D>().TargetPosition = target;
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
			var nextPosition = Parent.SnapToGround(GetComponent<NavigationAgent3D>().GetNextPathPosition());
			var direction = nextPosition - GlobalPosition;
			GetComponent<NavigationAgent3D>().Velocity = 1f * direction.Normalized();
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
