using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
namespace Project;

public partial class ObjectForcefulMovement : ComposableScript
{
	private List<Movement> Movements = new();

	public ObjectForcefulMovement(BaseUnit parent) : base(parent) { }

	public void Push(float distance, Vector3 direction, float timeInBeats)
	{
		var D = new Vector3(direction.X, Math.Max(0, direction.Y), direction.Z).Normalized() * distance;
		var t = timeInBeats * Music.Singleton.SecondsPerBeat;
		Movements.Add(new Movement()
		{
			Distance = D,
			Time = t,
			Speed = 2 * distance / t,
			Acceleration = -2 * distance / (t * t),
			CoveredDistance = 0,
		});
	}

	public override void _Process(double delta)
	{
		foreach (var movement in Movements)
			movement.ApplyToUnit(Parent, (float)delta);

		Movements = Movements.Where(movement => movement.CoveredDistanceSquared < movement.Distance.LengthSquared()).ToList();
	}

	public bool IsBeingMoved()
	{
		return Movements.Where(movement => movement.Speed > 1).ToList().Count > 0;
	}

	private class Movement
	{
		public float Time;
		public float Speed;
		public float Acceleration;
		public Vector3 Distance;
		public float CoveredDistance;
		public float CoveredDistanceSquared;

		public void ApplyToUnit(BaseUnit unit, float delta)
		{
			var distancePerFrame = Math.Min(Speed * delta, Distance.Length() - CoveredDistance);
			var movementVector = Distance.Normalized() * distancePerFrame;

			Speed = Math.Max(0, Speed + delta * Acceleration);
			unit.MoveAndCollide(movementVector);
			CoveredDistance += distancePerFrame;
			CoveredDistanceSquared = CoveredDistance * CoveredDistance;
		}
	}
}
