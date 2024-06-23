using Godot;
using System;

namespace Project;

public partial class SimpleParticleEffect : BaseEffect
{
	[Export]
	public GpuParticles3D Emitter;

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

	private async void StopAfterDelayAsync(float seconds)
	{
		await ToSignal(GetTree().CreateTimer(seconds), "timeout");
		Stop();
	}
}