using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Project;

public partial class Projectile : Node3D
{
	private bool IsEmitting = true;
	public BaseCast Source;
	public BaseUnit TargetUnit;
	public List<GpuParticles3D> Emitters;

	public float ImpactDamage = 5;
	public float FlightDuration = 1; // beat
	public Vector3 startingPosition;
	public float startingEngineTime = CastUtils.GetEngineTime();

	public string ImpactAudio = null;
	public string ImpactEffect = null;

	public override void _Ready()
	{
		Emitters = GetChildren().Where(child => child is GpuParticles3D).Cast<GpuParticles3D>().ToList();
	}

	public void Initialize(BaseCast source, BaseUnit targetUnit, Vector3 globalPosition, float damage, float flightDuration, string impactAudio = null, string impactEffect = null)
	{
		Source = source;
		TargetUnit = targetUnit;
		ImpactDamage = damage;
		FlightDuration = flightDuration;
		targetUnit.GetTree().CurrentScene.AddChild(this);
		GlobalPosition = globalPosition;
		startingPosition = globalPosition;
		ImpactAudio = impactAudio;
		ImpactEffect = impactEffect;
	}

	Node3D followSourceNode;
	bool followSource = false;

	public void FollowSource(Node3D sourceNode)
	{
		followSource = true;
		followSourceNode = sourceNode;
	}

	public override void _Process(double delta)
	{
		if (TargetUnit == null || !IsEmitting)
			return;

		if (!IsInstanceValid(TargetUnit))
		{
			CleanUp();
			return;
		}

		var engineTime = CastUtils.GetEngineTime();
		var flightDurationSeconds = FlightDuration * Music.Singleton.SecondsPerBeat;
		var targetPos = TargetUnit.GlobalCastAimPosition;

		var startPos = followSource ? followSourceNode.GlobalPosition : startingPosition;

		var time = (float)Math.Min((engineTime - startingEngineTime) / flightDurationSeconds, 1);
		Position = startPos + (targetPos - startPos) * time;

		if (time >= 1 && IsEmitting)
		{
			CleanUp();

			if (ImpactAudio != null)
				Audio.Play(ImpactAudio, GlobalPosition, 1.2f);

			if (ImpactEffect != null)
			{
				var impact = Lib.LoadScene(ImpactEffect).Instantiate<Node3D>();
				GetTree().Root.AddChild(impact);
				var scale = TargetUnit.Targetable.SelectionRadius;
				impact.Scale = new Vector3(scale, scale, scale);
				impact.GlobalPosition = GlobalPosition;
			}

			if (Source == null)
				GD.PushError("Projectile doesn't have Source set");

			TargetUnit.Health.Damage(ImpactDamage, Source);

			if (Source is Fireball && Source.HasSkill<SkillIgnitingFireball>())
			{
				TargetUnit.Buffs.Add(new BuffMinorIgnite()
				{
					SourceCast = Source,
				});
			}
		}
	}

	public async void CleanUp()
	{
		IsEmitting = false;
		foreach (var emitter in Emitters)
		{
			emitter.Emitting = false;
		}
		await ToSignal(GetTree().CreateTimer(2), "timeout");
		QueueFree();
	}
}