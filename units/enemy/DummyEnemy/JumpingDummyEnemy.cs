using Godot;
using System.Diagnostics;
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
		raisingTimer.WaitTime = 1.5f * Music.Singleton.BeatsPerSecond;
		raisingTimer.Timeout += OnRaisingTimerTick;

		restingTimer = GetNode<Timer>("RestingTimer");
		restingTimer.WaitTime = 0.5f * Music.Singleton.BeatsPerSecond;
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
		var circle = this.CreateGroundCircularArea(Position);
		circle.GrowTime = 1.5f;
		circle.Radius = 3;
		circle.Alliance = UnitAlliance.Hostile;
	}

	public void DoAttackDamage()
	{
		var attackRange = 3;
		var targets = AllUnits.Where(unit => unit.Alliance == UnitAlliance.Player && unit.Position.DistanceSquaredTo(Position) <= attackRange * attackRange && unit.Grounded);

		foreach (var target in targets)
		{
			target.Health.Damage(5);
			target.ForcefulMovement.Push(1, target.Position - Position, 1);
		}
	}
}
