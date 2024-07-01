using Godot;

namespace Project;

public partial class OnBeatEmitterLocalSpace : GpuParticles3D
{
	public override void _Ready()
	{
		Music.Singleton.BeatWindowUnlock += OnBeatTick;
		Emitting = false;
		EmitParticle(Transform, Vector3.Zero, new Color(1, 1, 1), new Color(1, 1, 1), 0);
		RandomizeColor();
	}

	public override void _ExitTree()
	{
		Music.Singleton.BeatWindowUnlock -= OnBeatTick;
	}

	public void OnBeatTick(BeatTime time)
	{
		if (time == BeatTime.Eighth || time == BeatTime.Sixteenth)
			return;
		EmitParticle(Transform, Vector3.Zero, new Color(1, 1, 1), new Color(1, 1, 1), 0);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("RandomizeColor".ToStringName()))
		{
			RandomizeColor();
		}
	}

	void RandomizeColor()
	{
		var color = new Color(GD.Randf(), GD.Randf(), GD.Randf());
		((StandardMaterial3D)DrawPass1.SurfaceGetMaterial(0)).AlbedoColor = color;
		((StandardMaterial3D)DrawPass1.SurfaceGetMaterial(0)).Emission = color;
		var light = GetParent().GetNode<OmniLight3D>("OmniLight3D");
		light.LightColor = color;
	}
}
