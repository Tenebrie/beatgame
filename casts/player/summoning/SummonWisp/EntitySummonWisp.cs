using Godot;
using System;

namespace Project;

public partial class EntitySummonWisp : Node3D
{
	public BaseUnit TargetUnit;
	public BaseCast SourceCast;
	public double PositionRadians;
	public float lifeDuration; // beats

	bool damageReady;
	Timer positioningTimer;
	Timer lifetimeTimer;

	public override void _Ready()
	{
		float prepDuration = 0.5f;
		positioningTimer = new() { WaitTime = prepDuration, OneShot = true };
		AddChild(positioningTimer);
		positioningTimer.Start();

		lifetimeTimer = new() { WaitTime = prepDuration + lifeDuration * Music.Singleton.SecondsPerBeat, OneShot = true };
		AddChild(lifetimeTimer);
		lifetimeTimer.Start();

		Music.Singleton.BeatTick += OnBeatTick;
		Music.Singleton.BeforeBeatTick()
	}

	public override void _ExitTree()
	{
		Music.Singleton.BeatTick -= OnBeatTick;
	}

	public override void _Process(double delta)
	{
		if (lifetimeTimer.IsStopped())
			return;

		if (!IsInstanceValid(TargetUnit))
		{
			lifetimeTimer.Stop();
			Cleanup();
			return;
		}
		var engineTime = CastUtils.GetEngineTime();

		var rotation = PositionRadians + engineTime * Music.Singleton.GameSpeed / 4 * Math.PI % 360;
		var offset = (Vector3.Forward.Rotated(Vector3.Up, (float)rotation) + Vector3.Up) * TargetUnit.Targetable.SelectionRadius * 1.5f;
		var targetPos = TargetUnit.GlobalCastAimPosition + offset;

		var velocity = (targetPos - GlobalPosition) * (float)delta * 5f;
		Position += velocity;
	}

	void OnBeatTick(BeatTime time)
	{
		if (time.IsNot(BeatTime.Whole | BeatTime.Half) || !positioningTimer.IsStopped() || lifetimeTimer.IsStopped())
			return;

		var projectile = Lib.LoadScene(Lib.Entity.SummonWispProjectile).Instantiate<Projectile>();
		projectile.Initialize(
			source: SourceCast,
			targetUnit: TargetUnit,
			globalPosition: GlobalPosition,
			damage: 5,
			flightDuration: 1,
			impactAudio: Lib.Audio.SfxMagicImpact01,
			impactEffect: Lib.Effect.FireballProjectileImpact
		);
		projectile.FollowSource(this);
		Audio.Play(Lib.Audio.SfxMagicLaunch01, GlobalPosition, 0.5f);
		if (lifetimeTimer.TimeLeft <= Music.Singleton.SecondsPerBeat * 2)
		{
			Music.Singleton.BeatTick -= OnBeatTick;
			Cleanup();
		}
	}

	void OnTimeTravelBeatTick(BeatTime time, Action<Action> queue)
	{
		if (time.IsNot(BeatTime.Whole | BeatTime.Half) || !positioningTimer.IsStopped() || lifetimeTimer.IsStopped())
			return;

		Audio.Play(Lib.Audio.SfxMagicLaunch01, GlobalPosition, 0.5f);
		queue(() =>
		{
			var projectile = Lib.LoadScene(Lib.Entity.SummonWispProjectile).Instantiate<Projectile>();
			projectile.Initialize(
				source: SourceCast,
				targetUnit: TargetUnit,
				globalPosition: GlobalPosition,
				damage: 5,
				flightDuration: 1,
				impactAudio: Lib.Audio.SfxMagicImpact01,
				impactEffect: Lib.Effect.FireballProjectileImpact
			);
			projectile.FollowSource(this);
			if (lifetimeTimer.TimeLeft <= Music.Singleton.SecondsPerBeat * 2)
			{
				Music.Singleton.BeatTick -= OnBeatTick;
				Cleanup();
			}
		});
	}

	async void Cleanup()
	{
		this.GetComponent<GpuParticles3D>().Emitting = false;
		await ToSignal(GetTree().CreateTimer(2), "timeout");
		QueueFree();
	}
}
