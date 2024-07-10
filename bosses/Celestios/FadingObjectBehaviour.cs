using Godot;
using System;

namespace Project;

public partial class FadingObjectBehaviour : Node
{
	[Export] MeshInstance3D mesh;

	public float fadeValue = 1;
	public float targetFadeValue = 1.05f;
	public float fadeSpeed = 1;

	public void SetFade(float value)
	{
		fadeValue = value;
		targetFadeValue = value;
		mesh.SetInstanceShaderParameter("FADE", value);
	}

	public void FadeIn(float duration)
	{
		targetFadeValue = 0;
		fadeSpeed = 1f / duration;
	}

	public void FadeOut(float duration)
	{
		targetFadeValue = 1.05f;
		fadeSpeed = 1f / duration;
	}

	public override void _Process(double delta)
	{
		if (fadeValue == targetFadeValue)
			return;

		if (targetFadeValue > fadeValue)
			fadeValue = Math.Min(fadeValue + fadeSpeed * (float)delta, targetFadeValue);
		if (targetFadeValue < fadeValue)
			fadeValue = Math.Max(fadeValue - fadeSpeed * (float)delta, targetFadeValue);
		mesh.SetInstanceShaderParameter("FADE", fadeValue);
	}
}
