using Godot;

namespace Project;

public partial class Preferences : Node
{
	private float mainVolume = 0.5f;
	public float MainVolume
	{
		get => mainVolume;
		set
		{
			mainVolume = value;
			Music.Singleton.CurrentTrack.Volume = value;
			SaveConfig();
		}
	}

	private float renderScale = 1f;
	public float RenderScale
	{
		get => renderScale;
		set
		{
			renderScale = value;
			GetViewport().Scaling3DScale = value;
			SaveConfig();
		}
	}

	public override void _EnterTree()
	{
		instance = this;
		LoadConfig();
		ApplyPreferences();
	}

	public void SaveConfig()
	{
		var config = new ConfigFile();

		config.SetValue("section", "mainVolume", MainVolume);

		config.Save("user://config.cfg");
	}

	public void LoadConfig()
	{
		var config = new ConfigFile();
		config.Load("user://config.cfg");

		mainVolume = (float)config.GetValue("section", "mainVolume", 0.5f);
		renderScale = (float)config.GetValue("section", "renderScale", GetDefaultRenderScale());
	}

	public void ApplyPreferences()
	{
		GetViewport().Scaling3DScale = renderScale;
	}

	private float GetDefaultRenderScale()
	{
		return OS.HasFeature("macos") ? 0.5f : 1;
	}

	private static Preferences instance = null;
	public static Preferences Singleton
	{
		get => instance;
	}
}