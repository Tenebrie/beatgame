using Godot;
using System;

namespace Project;
public partial class RectDecal : MeshInstance3D
{
	bool fadingIn = true;
	bool fadingOut = false;

	float fadeValue = 0;
	public Action onFadedOut;

	public override void _Ready()
	{
		SetInstanceShaderParameter("FADE", fadeValue);
		SetInstanceShaderParameter("CULL_DIST", this.GetArenaSize());
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

	public void CleanUp()
	{
		fadingIn = false;
		fadingOut = true;
	}
}
