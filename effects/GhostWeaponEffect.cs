using System;
using Godot;

namespace Project;

public partial class GhostWeaponEffect : BaseEffect
{
	[Export]
	public MeshInstance3D Mesh;

	float fadeValue = 1.05f;
	bool isSpawning = true;
	bool isCleaning = false;

	public Vector3 ForwardVector { get => -GlobalTransform.Basis.Z.Normalized(); }

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (isSpawning)
			fadeValue -= (float)delta * 2f;
		else if (isCleaning)
			fadeValue += (float)delta * 2f;

		if (isSpawning || isCleaning)
			UpdateFadeValue();

		if (isSpawning && fadeValue <= 0f)
			isSpawning = false;
		if (isCleaning && fadeValue >= 1.05f)
			QueueFree();
	}

	void UpdateFadeValue()
	{
		Mesh.SetInstanceShaderParameter("FADE", fadeValue);
	}

	public void SetVisualVelocity(Vector3 velocity, float maxStretch = Mathf.Inf)
	{
		var vel = velocity.Normalized() * Math.Min(maxStretch, velocity.Length());
		Mesh.SetInstanceShaderParameter("VELOCITY", vel);
	}

	public void CleanUp()
	{
		isCleaning = true;
	}
}