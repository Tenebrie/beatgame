using Godot;
using System.Linq;

namespace Project;

public partial class JumpingDummyEnemy : BasicEnemyController
{
	enum State
	{
		Raising,
		Falling,
		Resting,
	}

	State state = State.Resting;
	float fallSpeed = 0;

	Timer raisingTimer;
	Timer restingTimer;

	public JumpingDummyEnemy()
	{
		FriendlyName = "Jumping Dummy Enemy";
	}

	public override void _Ready()
	{
		base._Ready();

		raisingTimer = GetNode<Timer>("RaisingTimer");
		raisingTimer.Timeout += OnRaisingTimerTick;
		restingTimer = GetNode<Timer>("RestingTimer");
		restingTimer.Timeout += OnRestingTimerTick;
		restingTimer.Start();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (Grounded && state == State.Falling)
		{
			DoAttackDamage();
			state = State.Resting;
			restingTimer.Start();
		}
	}

	public void OnRaisingTimerTick()
	{
		state = State.Falling;
		Gravity = BaseGravity;
		Velocity = new Vector3(Velocity.X, -10, Velocity.Z);
	}

	public void OnRestingTimerTick()
	{
		state = State.Raising;
		raisingTimer.Start();
		Gravity = -BaseGravity / 10;
	}

	public void DoAttackDamage()
	{
		var attackRange = 3;
		var targets = AllUnits.Where(unit => unit.Alliance == UnitAlliance.Player && unit.Position.DistanceSquaredTo(Position) <= attackRange * attackRange && unit.Grounded);

		foreach (var target in targets)
		{
			target.Health.Damage(15);
		}
	}
}
