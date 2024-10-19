using Godot;
using System.Linq;

namespace Project;

public partial class GiantRatBehaviour : BaseBehaviour
{
	EnemyAIBehaviour enemyAI;
	NavigationAgent3D navAgent;

	AttackCast attackCast;
	RangedAttackCast rangedAttackCast;

	AIState State
	{
		get => GetComponent<EnemyAIBehaviour>().State;
		set => GetComponent<EnemyAIBehaviour>().State = value;
	}

	public override void _Ready()
	{
		Parent.FriendlyName = "Giant Rat";

		attackCast = new(Parent);
		AddChild(attackCast);
		rangedAttackCast = new(Parent);
		AddChild(rangedAttackCast);

		GetComponent<AnimationController>().RegisterStateTransitions(
			("Bite", (_) => State == AIState.Attacking),
			("Walk", (_) => State is AIState.Wandering or AIState.EnemySpotted),
			("Idle", (_) => true)
		);

		enemyAI = GetComponent<EnemyAIBehaviour>();
		enemyAI.AgentMaxSpeed = 2f;
		enemyAI.UpdateTick += OnAIUpdate;
	}

	void OnAIUpdate()
	{
		if (enemyAI.TargetEnemy == null)
		{
			// TODO: Optimize
			var targets = BaseUnit.AllUnits
				.Where(u => u.HostileTo(Parent))
				.Select(u => (unit: u, distance: u.GlobalPosition.DistanceTo(Parent.GlobalPosition)))
				.Where(tuple => tuple.distance <= 7)
				.OrderBy(a => a.distance);

			var (unit, _) = targets.FirstOrDefault();
			if (unit == null)
				return;

			enemyAI.TargetEnemy = unit;
		}

		var directDistance = enemyAI.TargetEnemy.GlobalPosition.DistanceTo(Parent.GlobalPosition);
		var distance = enemyAI.PathDistanceTo(enemyAI.TargetEnemy.GlobalPosition);
		this.Log($"Direct distance: {directDistance}, Path distance: {distance}");

		if (directDistance <= 1.5f)
		{
			enemyAI.MakeBusy(2f);
			State = AIState.Attacking;
			var castTargetData = new CastTargetData()
			{
				HostileUnit = enemyAI.TargetEnemy,
			};
			if (attackCast.ValidateIfCastIsPossible(castTargetData, out _) is BaseCast.CastQueueMode.Instant)
			{
				attackCast.CastBegin(castTargetData);
			}
		}
		else if (distance >= 7 && directDistance <= 5)
		{
			enemyAI.MakeBusy(2f);
			State = AIState.Attacking;
			var castTargetData = new CastTargetData()
			{
				HostileUnit = enemyAI.TargetEnemy,
			};
			if (rangedAttackCast.ValidateIfCastIsPossible(castTargetData, out _) is BaseCast.CastQueueMode.Instant)
			{
				rangedAttackCast.CastBegin(castTargetData);
			}
		}
		else
		{
			State = AIState.EnemySpotted;
		}
	}

	public override void _Process(double delta)
	{
		if (State is AIState.EnemySpotted && enemyAI.TargetEnemy != null)
		{
			enemyAI.PathTowards(enemyAI.TargetEnemy.GlobalPosition);
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

partial class RangedAttackCast : BaseCast
{
	public RangedAttackCast(BaseUnit parent) : base(parent)
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
		this.CreateZapEffect(Parent.GlobalCastAimPosition, targetData.HostileUnit.GlobalCastAimPosition);
	}
}
