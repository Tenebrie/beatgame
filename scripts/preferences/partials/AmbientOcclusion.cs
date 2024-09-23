using Godot;
using Project;

namespace BeatGame.scripts.preferences;

public partial class Preferences : Node
{
	public void ApplyAmbientOcclusion()
	{
		var environment = EnvironmentController.Singleton;
		if (environment != null && environment.WorldEnvironment != null)
		{
			environment.WorldEnvironment.Environment.SsaoEnabled = AmbientOcclusion;
		}
	}
}