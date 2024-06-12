using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace Project;

public abstract partial class BaseUnit : ComposableCharacterBody3D
{
	public string FriendlyName = "Unnamed unit";
	public ObjectResource Health;
	public ObjectResource Mana;
	public ObjectBuffs Buffs;
	public ObjectTargetable Targetable;
	public ObjectForcefulMovement ForcefulMovement;
	public ObjectCastLibrary CastLibrary;

	[Export]
	public UnitAlliance Alliance = UnitAlliance.Neutral;

	public const float TerminalVelocity = -30f;
	public float BaseGravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	public float Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	public bool Grounded = true;
	private int FramesInFlight = 0;

	public bool IsBeingMoved { get => ForcefulMovement.IsBeingMoved(); }

	public bool IsAlive = true;
	public bool IsDead { get => !IsAlive; }

	public BaseUnit()
	{
		Health = new ObjectResource(this, ObjectResourceType.Health, max: 100);
		Mana = new ObjectResource(this, ObjectResourceType.Mana, max: 0);
		Buffs = new ObjectBuffs(this);
		Targetable = new ObjectTargetable(this);
		ForcefulMovement = new ObjectForcefulMovement(this);
		CastLibrary = new ObjectCastLibrary(this);

		Composables.Add(Buffs);
		Composables.Add(Health);
		Composables.Add(Mana);
		Composables.Add(Targetable);
		Composables.Add(ForcefulMovement);
		Composables.Add(CastLibrary);
	}

	public Vector3 ForwardVector { get => -GlobalTransform.Basis.Z.Normalized(); }
	public Vector3 CastAimPosition { get => new(Position.X, Position.Y + 0.25f, Position.Z); }

	public override void _Ready()
	{
		base._Ready();

		SignalBus.Singleton.ResourceChanged += OnResourceChanged;

		AllUnits.Add(this);
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.UnitCreated, this);
	}

	private void OnResourceChanged(BaseUnit unit, ObjectResourceType type, float value)
	{
		if (unit != this || type != ObjectResourceType.Health || value > 0 || IsDead)
			return;

		Instakill();
	}

	public void Instakill()
	{
		if (IsDead)
			return;

		IsAlive = false;
		QueueFree();
	}

	public override void _Process(double delta)
	{
		ProcessInertia(delta);
		ProcessGravity(delta);
		base._Process(delta);
	}

	public override void _ExitTree()
	{
		AllUnits.Remove(this);
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.UnitDestroyed, this);
	}

	protected virtual void ProcessInertia(double delta)
	{
		Velocity = new Vector3(
			x: Velocity.X,
			y: Velocity.Y,
			z: Velocity.Z
		);

		MoveAndCollide(new Vector3(Velocity.X, 0, Velocity.Z) * (float)delta);

		var newVector = Velocity - 10.00f * new Vector3(Velocity.X, 0, Velocity.Z).Normalized() * (float)delta;
		Velocity = new Vector3(newVector.X, Velocity.Y, newVector.Z);
	}

	protected virtual void ProcessGravity(double delta)
	{
		var verticalVelocity = Math.Max(TerminalVelocity, Velocity.Y - Gravity * (float)delta);

		Velocity = new Vector3(
			x: Velocity.X,
			y: verticalVelocity,
			z: Velocity.Z
		);

		if (Velocity.Y == 0)
			return;

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
		if (Position.Y <= -30)
		{
			Position = new Vector3(0, 1, 0);
		}
	}

	public static readonly List<BaseUnit> AllUnits = new();
}
