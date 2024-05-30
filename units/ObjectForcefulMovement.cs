using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
namespace Project;

public partial class ObjectForcefulMovement : ComposableScript
{
	private List<Movement> Movements = new();
	private List<ContinuousMovement> ContinuousMovements = new();

	public ObjectForcefulMovement(BaseUnit parent) : base(parent) { }

	public void Push(float distance, Vector3 direction, float timeInBeats)
	{
		var D = new Vector3(direction.X, Math.Max(0, direction.Y), direction.Z).Normalized() * distance;
		var t = timeInBeats * Music.Singleton.SecondsPerBeat;
		Movements.Add(new Movement()
		{
			Unit = Parent,
			Distance = D,
			Time = t,
			Speed = 2 * distance / t,
			Acceleration = -2 * distance / (t * t),
			CoveredDistance = 0,
		});
	}

	public ContinuousMovement PushContinuously(float speed, Vector3 direction)
	{
		var movement = new ContinuousMovement()
		{
			Unit = Parent,
			Speed = speed,
			Direction = direction.Normalized(),
		};
		ContinuousMovements.Add(movement);
		return movement;
	}

	public override void _Process(double delta)
	{
		foreach (var movement in Movements)
			movement.Apply((float)delta);

		Movements = Movements.Where(movement => movement.CoveredDistanceSquared < movement.Distance.LengthSquared()).ToList();

		foreach (var movement in ContinuousMovements)
			movement.Apply((float)delta);
	}

	public bool IsBeingMoved()
	{
		return Movements.Where(movement => movement.Speed > 1).ToList().Count > 0;
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
		public float Speed;
		public Vector3 Direction;

		public void Apply(float delta)
		{
			var distancePerFrame = Speed * delta;
			var movementVector = Direction * distancePerFrame;
			Unit.MoveAndCollide(movementVector);
		}

		public void Stop()
		{
			Unit.ForcefulMovement.ContinuousMovements.Remove(this);
		}
	}
}
