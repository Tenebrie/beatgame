using Godot;

namespace Project;

[Tool]
public partial class DemoLightningZapEffect : Node3D
{
	MeshInstance3D Target;
	LightningZapEffect Zap;
	float UpdateTimer = 0;
	public override void _Ready()
	{
		Target = GetNode<MeshInstance3D>("Target");
	}

	public override void _Process(double delta)
	{
		UpdateTimer += (float)delta;
		if (UpdateTimer >= 1)
		{
			UpdateTimer = 0;
			Zap = GD.Load<PackedScene>("res://effects/LightningZap/LightningZapEffect.tscn").Instantiate<LightningZapEffect>();
			AddChild(Zap);
			Zap.SetTarget(Target.Position);
		}
	}
}
