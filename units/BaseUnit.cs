using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace Project;

public abstract partial class BaseUnit : CharacterBody3D
{
	public ObjectResource Health;
	public ObjectTargetable Targetable;

	public UnitAlliance Alliance = UnitAlliance.Neutral;
	public List<ComposableScript> Composables = new();

	public const float TerminalVelocity = -30f;
	readonly public float BaseGravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	public float Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	public bool Grounded = true;
	private int FramesInFlight = 0;

	public BaseUnit()
	{
		Health = new ObjectResource(this, ObjectResourceType.Health, max: 100);
		Targetable = new ObjectTargetable(this);

		Composables.Add(Health);
		Composables.Add(Targetable);
	}

	public override void _Ready()
	{
		AllUnits.Add(this);
		foreach (var composable in Composables)
		{
			composable._Ready();
		}
	}

	public override void _Process(double delta)
	{
		ProcessGravity(delta);
		foreach (var composable in Composables)
		{
			composable._Process(delta);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		// ProcessGravity(delta);
	}

	protected virtual void ProcessGravity(double delta)
	{
		var verticalVelocity = Math.Max(TerminalVelocity, Velocity.Y - Gravity * (float)delta);

		Velocity = new Vector3(
			x: Velocity.X,
			y: verticalVelocity,
			z: Velocity.Z
		);

		var collision = MoveAndCollide(new Vector3(0, Velocity.Y * (float)delta, 0));

		var hitGround = collision != null;

		if (hitGround)
		{
			Velocity = new Vector3(Velocity.X, 0, Velocity.Z);
			FramesInFlight = 0;
		}
		else
			FramesInFlight += 1;

		Grounded = FramesInFlight <= Math.Ceiling(Engine.GetFramesPerSecond() / 40);

		// Out of bounds plane snap
		if (Position.Y <= -20)
		{
			Position = new Vector3(0, 1, 0);
		}
	}

	public override void _Input(InputEvent @event)
	{
		foreach (var composable in Composables)
		{
			composable._Input(@event);
		}
	}

	public override void _ExitTree()
	{
		foreach (var composable in Composables)
		{
			composable._ExitTree();
		}
	}

	public static readonly List<BaseUnit> AllUnits = new();
}
