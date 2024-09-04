using Godot;
using System;

namespace Project;

public partial class SimpleParticleEffect : BaseEffect
{
	public GpuParticles3D Emitter;

	public override void _Ready()
	{
		base._Ready();
		Emitter = this.GetComponent<GpuParticles3D>();
		Emitter.Restart();
	}

	public SimpleParticleEffect Stop()
	{
		Emitter.Emitting = false;
		return this;
	}

	public override SimpleParticleEffect SetLifetime(float seconds)
	{
		StopAfterDelayAsync(seconds);
		base.SetLifetime(seconds + 4);
		return this;
	}

	protected async void StopAfterDelayAsync(float seconds)
	{
		await ToSignal(GetTree().CreateTimer(seconds), "timeout");
		Stop();
	}
}
