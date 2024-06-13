using Godot;
using System;
using System.Collections.Generic;

namespace Project;

public partial class EffectFlamethrowerWithHitbox : Area3D
{
	public float DamagePerBeat;
	public BaseUnit FollowedUnit;
	public BaseUnit TargetUnit;
	public BaseCast SourceCast;
	public Func<BaseUnit, bool> TargetValidator;

	readonly List<BaseUnit> AffectedUnits = new();
	CircleDecal decal;
	EffectFlamethrower particles;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
		Music.Singleton.BeatTick += OnBeatTick;

		decal = GetNode<CircleDecal>("CircleDecal");
		particles = GetNode<EffectFlamethrower>("EffectFlamethrower");
	}

	public override void _ExitTree()
	{
		BodyEntered -= OnBodyEntered;
		BodyExited -= OnBodyExited;
		Music.Singleton.BeatTick -= OnBeatTick;
	}

	void OnBodyEntered(Node3D body)
	{
		if (body is not BaseUnit unit)
			return;

		if (TargetValidator(unit))
			AffectedUnits.Add(unit);
	}

	void OnBodyExited(Node3D body)
	{
		if (body is not BaseUnit unit)
			return;

		AffectedUnits.Remove(unit);
	}

	void OnBeatTick(BeatTime time)
	{
		if (!particles.Fire.Emitting)
			return;

		foreach (var unit in AffectedUnits)
			unit.Health.Damage(DamagePerBeat * Music.MinBeatSize, SourceCast);
	}

	public override void _Process(double delta)
	{
		if (!particles.Fire.Emitting)
			return;

		if (!IsInstanceValid(FollowedUnit) || !IsInstanceValid(TargetUnit))
		{
			CleanUp();
			return;
		}

		LookAt(TargetUnit.GlobalCastAimPosition);
		decal.GlobalPosition = this.SnapToGround(FollowedUnit.GlobalPosition);
		decal.GlobalRotation = new Vector3(0, decal.GlobalRotation.Y, 0);
	}

	public async void CleanUp()
	{
		particles.Fire.Emitting = false;
		particles.Smoke.Emitting = false;
		particles.Embers.Emitting = false;
		decal.CleanUp();
		await ToSignal(GetTree().CreateTimer(3), "timeout");
		QueueFree();
	}
}
