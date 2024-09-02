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
	}

	void OnAIUpdate()
	{
		// Agent is locked into an action, do not update
		if (!stateLockTimer.IsStopped())
			return;

		// Agent is returning to home position after being moved too far away
		if (state is AIState.Leashing && Parent.GlobalPosition.DistanceTo(homePosition) >= 1)
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
			var direction = CastUtils.SnapToGround(Parent, lockedOnTarget.GlobalPosition) - CastUtils.SnapToGround(Parent, Parent.GlobalPosition);
			Parent.Velocity = 1f * direction.Normalized();
			Parent.MoveAndSlide();
			Parent.LookAt(lockedOnTarget.GlobalPosition);
		}
		else if (state is AIState.Leashing)
		{
			var direction = CastUtils.SnapToGround(Parent, homePosition) - CastUtils.SnapToGround(Parent, Parent.GlobalPosition);
			Parent.Velocity = 1f * direction.Normalized();
			Parent.MoveAndSlide();
			Parent.LookAt(homePosition);
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
