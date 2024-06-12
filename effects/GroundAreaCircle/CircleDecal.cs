using Godot;
using System;

namespace Project;
public partial class CircleDecal : MeshInstance3D
{
	bool fadingIn = true;
	bool fadingOut = false;

	float fadeValue = 0;
	public Action onFadedOut;

	public override void _Ready()
	{
		SetInstanceShaderParameter("FADE", fadeValue);
	}

	public override void _Process(double delta)
	{
		if (fadingOut)
		{
			fadeValue -= (float)delta * 4;

			SetInstanceShaderParameter("FADE", fadeValue);
			if (fadeValue <= 0)
			{
				onFadedOut?.Invoke();
			}
		}
		else if (fadingIn)
		{
			fadeValue += (float)delta * 4;
			SetInstanceShaderParameter("FADE", fadeValue);
			if (fadeValue >= 1)
			{
				fadeValue = 1;
				fadingIn = false;
			}
		}
	}

	public void EnableCulling()
	{
		SetInstanceShaderParameter("CULL_DIST", this.GetArenaSize());
	}

	public void SetRadius(float radius)
	{
		SetInstanceShaderParameter("RADIUS", radius);
	}

	public void SetInnerAlpha(float value)
	{
		SetInstanceShaderParameter("INNER_ALPHA", value);
	}

	public void SetProgress(float value)
	{
		SetInstanceShaderParameter("PROGRESS", value);
	}

	public void SetColor(Color color)
	{
		SetInstanceShaderParameter("COLOR_R", color.R);
		SetInstanceShaderParameter("COLOR_G", color.G);
		SetInstanceShaderParameter("COLOR_B", color.B);
	}

	public void CleanUp()
	{
		fadeValue = 1;
		fadingIn = false;
		fadingOut = true;
	}
}
