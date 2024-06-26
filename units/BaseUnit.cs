using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace Project;

public abstract partial class BaseUnit : ComposableCharacterBody3D
{
	[Signal] public delegate void UnitKilledEventHandler();
	[Signal] public delegate void UnitDestroyedEventHandler();

	public string FriendlyName = "Unnamed unit";
	public ObjectResource Health;
	public ObjectResource Mana;
	public ObjectBuffs Buffs;
	public ObjectTargetable Targetable;
	public ObjectForcefulMovement ForcefulMovement;
	public ObjectCastLibrary CastLibrary;
	public ObjectComponentLibrary Components;

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

	public Vector3 ForwardVector { get => -GlobalTransform.Basis.Z.Normalized(); }

	public Vector3 CastAimPosition = new();
	public Vector3 GlobalCastAimPosition { get => GlobalPosition + CastAimPosition; }

	public override void _Ready()
	{
		base._Ready();

		Health = new(this, ObjectResourceType.Health, max: 100);
		Mana = new(this, ObjectResourceType.Mana, max: 0);
		Buffs = new(this);
		Targetable = new(this);
		ForcefulMovement = new(this);
		CastLibrary = new(this);
		Components = new(this);

		AddChild(Buffs);
		AddChild(Health);
		AddChild(Mana);
		AddChild(Targetable);
		AddChild(ForcefulMovement);
		AddChild(CastLibrary);
		AddChild(Components);
		AddChild(new ObjectReactions(this));

		SignalBus.Singleton.ResourceChanged += OnResourceChanged;

		AllUnits.Add(this);
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.UnitCreated, this);
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		Music.Singleton.BeatTick += ProcessBeatTick;
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
		HandleDeath();
		EmitSignal(SignalName.UnitKilled);
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.UnitKilled, this);
	}

	protected virtual void HandleDeath()
	{
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
		base._ExitTree();
		Music.Singleton.BeatTick -= ProcessBeatTick;
		AllUnits.Remove(this);
		EmitSignal(SignalName.UnitDestroyed);
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
		if (newVector.LengthSquared() <= 0.05f)
			Velocity = new Vector3(0, Velocity.Y, 0);
		else
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

	protected virtual void ProcessBeatTick(BeatTime time) { }

	public static readonly List<BaseUnit> AllUnits = new();
}
