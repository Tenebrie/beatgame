using System.Diagnostics;
using Godot;

namespace BeatGame.scripts.preferences;

public enum FogQuality : int
{
	Low = 0,
	Medium = 1,
	High = 2,
	Ultra = 3,
}

public partial class Preferences : Node
{
	public void ApplyFogQuality()
	{
		var fogSize = 0;
		var fogDepth = 0;
		switch (FogQualityLevel)
		{
			case FogQuality.Low:
				fogSize = 64;
				fogDepth = 64;
				break;
			case FogQuality.Medium:
				fogSize = 96;
				fogDepth = 64;
				break;
			case FogQuality.High:
				fogSize = 128;
				fogDepth = 64;
				break;
			case FogQuality.Ultra:
				fogSize = 192;
				fogDepth = 92;
				break;
		}

		RenderingServer.Singleton.EnvironmentSetVolumetricFogVolumeSize(fogSize, fogDepth);
	}
}