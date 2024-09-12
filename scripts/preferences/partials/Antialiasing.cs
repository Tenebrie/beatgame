using Godot;

namespace BeatGame.scripts.preferences;

public enum Antialiasing : int
{
	None = 0,
	FXAA = 1,
	TAA = 2,
	MSAA_2 = 3,
	MSAA_4 = 4,
	MSAA_8 = 5,
	ALL = 6,
}

public partial class Preferences : Node
{
	public void ApplyAntialiasing()
	{
		var viewport = GetViewport();
		viewport.UseTaa = false;
		viewport.Msaa3D = Viewport.Msaa.Disabled;
		viewport.ScreenSpaceAA = Viewport.ScreenSpaceAAEnum.Disabled;
		switch (AntialiasingLevel)
		{
			case Antialiasing.None:
				viewport.Msaa3D = Viewport.Msaa.Disabled;
				break;
			case Antialiasing.FXAA:
				viewport.ScreenSpaceAA = Viewport.ScreenSpaceAAEnum.Fxaa;
				break;
			case Antialiasing.TAA:
				viewport.UseTaa = true;
				break;
			case Antialiasing.MSAA_2:
				viewport.Msaa3D = Viewport.Msaa.Msaa2X;
				break;
			case Antialiasing.MSAA_4:
				viewport.Msaa3D = Viewport.Msaa.Msaa4X;
				break;
			case Antialiasing.MSAA_8:
				viewport.Msaa3D = Viewport.Msaa.Msaa8X;
				break;
			case Antialiasing.ALL:
				viewport.UseTaa = true;
				viewport.Msaa3D = Viewport.Msaa.Msaa8X;
				viewport.ScreenSpaceAA = Viewport.ScreenSpaceAAEnum.Fxaa;
				break;
		}
	}
}