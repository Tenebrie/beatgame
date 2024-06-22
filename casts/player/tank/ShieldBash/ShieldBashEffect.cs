using Godot;
using Project;
using System;

namespace Project;

public partial class ShieldBashEffect : GhostWeaponEffect
{
	[Export]
	Timer timer;

	State state = State.Spawning;
	Vector3 velocity = new();

	[Signal]
	public delegate void ImpactEventHandler();

	public float DistanceToTarget;

	void OnStateTransition(State transitionTo)
	{
		state = transitionTo;
		if (state == State.Accelerating)
		{
			velocity = ForwardVector * (DistanceToTarget - 0.25f) * Music.Singleton.BeatsPerSecond / 0.25f;
		}
		if (state == State.Despawning)
		{
			velocity = new();
			EmitSignal(SignalName.Impact);
			CleanUp();
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		var timePassed = (timer.WaitTime - timer.TimeLeft) * Music.Singleton.BeatsPerSecond;
		if (state == State.Spawning && timePassed >= 0.75f)
			OnStateTransition(State.Accelerating);
		if (state == State.Accelerating && timePassed >= 1.0f)
			OnStateTransition(State.Despawning);

		if (state == State.Accelerating)
		{
			Position += velocity * (float)delta;
		}
		if (state == State.Despawning)
		{
			float velocityLength = velocity.Length();
			if (velocityLength - (float)delta * 10f < 0)
				velocity = new();
			else
				velocity -= ForwardVector * (float)delta * 10f;
		}
	}

	enum State
	{
		Spawning,
		Accelerating,
		Despawning,
	}
}
