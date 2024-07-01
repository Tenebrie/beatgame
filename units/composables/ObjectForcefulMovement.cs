using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Project;

public partial class ObjectForcefulMovement : ComposableScript
{
	private List<Movement> Movements = new();
	private readonly List<ContinuousMovement> ContinuousMovements = new();

	public ObjectForcefulMovement(BaseUnit parent) : base(parent) { }

	public void Push(float distance, Vector3 direction, float timeInBeats, bool inverted = false)
	{
		distance *= 1 - Parent.Buffs.State.PercentageCCReduction;

		var D = new Vector3(direction.X, direction.Y, direction.Z).Normalized() * distance;
		var t = timeInBeats * Music.Singleton.SecondsPerBeat;

		var speed = 2 * distance / t;
		var acceleration = -2 * distance / (t * t);
		if (inverted)
		{
			speed = 0;
			acceleration = -acceleration;
		}
		Movements.Add(new Movement()
		{
			Unit = Parent,
			Distance = D,
			Time = t,
			Speed = speed,
			Acceleration = acceleration,
			CoveredDistance = 0,
		});
	}

	public void AddInertia(float inertia, Vector3 direction)
	{
		Parent.Velocity += direction.Normalized() * inertia;
	}

	public ContinuousMovement PushContinuously(Func<float> speed, Func<Vector3> direction, Func<bool> condition = null)
	{
		var movement = new ContinuousMovement()
		{
			Unit = Parent,
			Speed = speed,
			Direction = direction,
			Condition = condition,
		};
		ContinuousMovements.Add(movement);
		return movement;
	}

	public override void _Process(double delta)
	{
		for (var i = 0; i < Movements.Count; i++)
			Movements[i].Apply((float)delta);

		Movements.RemoveAll(movement => movement.CoveredDistanceSquared >= movement.Distance.LengthSquared());

		for (var i = 0; i < ContinuousMovements.Count; i++)
			ContinuousMovements[i].Apply((float)delta);
	}

	private class Movement
	{
		public BaseUnit Unit;
		public float Time;
		public float Speed;
		public float Acceleration;
		public Vector3 Distance;
		public float CoveredDistance;
		public float CoveredDistanceSquared;

		public void Apply(float delta)
		{
			var distancePerFrame = Math.Min(Speed * delta, Distance.Length() - CoveredDistance);
			var movementVector = Distance.Normalized() * distancePerFrame;

			Speed = Math.Max(0, Speed + delta * Acceleration);
			Unit.MoveAndCollide(movementVector);
			CoveredDistance += distancePerFrame;
			CoveredDistanceSquared = CoveredDistance * CoveredDistance;
		}
	}

	public class ContinuousMovement
	{
		public string ID = Guid.NewGuid().ToString();
		public BaseUnit Unit;
		public Func<float> Speed;
		public Func<Vector3> Direction;
		public Func<bool> Condition;

		public void Apply(float delta)
		{
			var isActive = Condition?.Invoke();
			if (isActive == false)
				return;

			var distancePerFrame = Speed() * delta * (1 - Unit.Buffs.State.PercentageCCReduction);
			var movementVector = Direction().Normalized() * distancePerFrame;
			Unit.MoveAndCollide(movementVector);
		}

		public void Stop()
		{
			Unit.ForcefulMovement.ContinuousMovements.Remove(this);
		}
	}
}
