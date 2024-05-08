using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace Project;

public abstract partial class BaseUnit : BaseComposable
{
	public string FriendlyName = "Unnamed unit";
	public ObjectResource Health;
	public ObjectTargetable Targetable;

	public UnitAlliance Alliance = UnitAlliance.Neutral;

	public const float TerminalVelocity = -30f;
	readonly public float BaseGravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	public float Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	public bool Grounded = true;
	private int FramesInFlight = 0;

	private bool IsAlive = true;
	private bool IsDead { get => !IsAlive; }

	public BaseUnit()
	{
		Health = new ObjectResource(this, ObjectResourceType.Health, max: 100);
		Targetable = new ObjectTargetable(this);

		Composables.Add(Health);
		Composables.Add(Targetable);
	}

	public Vector3 ForwardVector { get => -GlobalTransform.Basis.Z; }

	public override void _Ready()
	{
		base._Ready();

		SignalBus.GetInstance(this).ResourceChanged += OnResourceChanged;

		AllUnits.Add(this);
		SignalBus.GetInstance(this).EmitSignal(SignalBus.SignalName.UnitCreated, this);
	}

	private void OnResourceChanged(BaseUnit unit, ObjectResourceType type, float value)
	{
		if (unit != this || type != ObjectResourceType.Health || value > 0 || IsDead)
			return;

		IsAlive = false;
		QueueFree();
		AllUnits.Remove(this);
		SignalBus.GetInstance(this).EmitSignal(SignalBus.SignalName.UnitDestroyed, this);
	}

	public override void _Process(double delta)
	{
		ProcessGravity(delta);
		base._Process(delta);
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

	public static readonly List<BaseUnit> AllUnits = new();
}
