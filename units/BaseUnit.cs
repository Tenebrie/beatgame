using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace Project;

[GlobalClass]
public partial class BaseUnit : CharacterBody3D
{
	[Signal] public delegate void UnitKilledEventHandler();
	[Signal] public delegate void UnitDestroyedEventHandler();

	public string FriendlyName = "Unnamed unit";
	public ObjectResource Health;
	public ObjectResource Mana;
	public ObjectBuffs Buffs;
	public ObjectStats Stats;
	public ObjectTargetable Targetable;
	public ObjectForcefulMovement ForcefulMovement;
	public ObjectCastLibrary CastLibrary;
	public ObjectNameplate Nameplate;

	public ObjectComponentLibrary Components;

	[Export] public UnitAlliance Alliance = UnitAlliance.Neutral;

	public const float TerminalVelocity = -30f;
	public float BaseGravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	public float Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	public bool Grounded = true;
	private int FramesInFlight = 0;

	public bool IsAlive = true;
	public bool IsDead { get => !IsAlive; }

	public Vector3 ForwardVector { get => -GlobalTransform.Basis.Z.Normalized(); }

	public Vector3 CastAimPosition = new();
	public Vector3 GlobalCastAimPosition { get => GlobalPosition + CastAimPosition; }

	public override void _EnterTree()
	{
		Health = new(this, ObjectResourceType.Health, max: 100);
		Mana = new(this, ObjectResourceType.Mana, max: 0);
		Buffs = new(this);
		Stats = new(this);
		Targetable = new(this);
		ForcefulMovement = new(this);
		CastLibrary = new(this);
		Nameplate = new();

		Components = new(this);
		Music.Singleton.BeatTick += ProcessBeatTick;
	}

	public override void _Ready()
	{
		// All non-player units are hoverable
		if (this is not PlayerController)
			CollisionLayer += Raycast.Layer.Hoverable.AsUnsignedInt();

		AddChild(Buffs);
		AddChild(Health);
		AddChild(Mana);
		AddChild(Targetable);
		AddChild(ForcefulMovement);
		AddChild(CastLibrary);
		AddChild(Components);
		AddChild(Nameplate);

		AddChild(new ObjectReactions(this));

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
		var verticalVelocity = Velocity.Y - Gravity * (float)delta * 1.2f;
		Velocity = new Vector3(
			x: Velocity.X,
			y: verticalVelocity,
			z: Velocity.Z
		);

		if (Velocity.LengthSquared() <= 0.005f)
			return;

		MoveAndSlide();

		Grounded = IsOnFloor();
		Velocity -= 2.00f * new Vector3(Velocity.X, Velocity.Y, Velocity.Z).Normalized() * (float)delta;

		if (Position.Y <= -30)
		{
			Position = new Vector3(0, 1, 0);
		}
	}

	protected virtual void ProcessBeatTick(BeatTime time) { }

	public static readonly List<BaseUnit> AllUnits = new();
}
