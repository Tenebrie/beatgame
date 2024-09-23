using Godot;

namespace BeatGame.scripts.preferences;

public partial class Preferences : Node
{
	public void ApplyRenderScale()
	{
		GetViewport().Scaling3DScale = RenderScale;
	}
}