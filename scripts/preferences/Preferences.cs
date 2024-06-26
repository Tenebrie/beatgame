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
			AudioServer.SetBusVolumeDb(0, Mathf.LinearToDb(value));
			SaveConfig();
		}
	}

	private float musicVolume = 1.0f;
	public float MusicVolume
	{
		get => musicVolume;
		set
		{
			musicVolume = value;
			Music.Singleton.CurrentTrack.Volume = value;
			SaveConfig();
		}
	}

	private float cameraHeight = 0;
	public float CameraHeight
	{
		get => cameraHeight;
		set
		{
			cameraHeight = value;
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

	private bool chillMode = true;
	public bool ChillMode
	{
		get => chillMode;
		set
		{
			chillMode = value;
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
		config.SetValue("section", "musicVolume", musicVolume);
		config.SetValue("section", "cameraHeight", CameraHeight);
		config.SetValue("section", "chillMode", ChillMode);

		config.Save("user://config.cfg");
	}

	public void LoadConfig()
	{
		var config = new ConfigFile();
		config.Load("user://config.cfg");

		mainVolume = (float)config.GetValue("section", "mainVolume", 0.5f);
		musicVolume = (float)config.GetValue("section", "musicVolume", 1.0f);
		cameraHeight = (float)config.GetValue("section", "cameraHeight", 0.25f);
		chillMode = (bool)config.GetValue("section", "chillMode", true);

		renderScale = (float)config.GetValue("section", "renderScale", GetDefaultRenderScale());
	}

	public void ApplyPreferences()
	{
		AudioServer.SetBusVolumeDb(0, Mathf.LinearToDb(mainVolume));
		GetViewport().Scaling3DScale = renderScale;
	}

	private static float GetDefaultRenderScale()
	{
		return OS.HasFeature("macos") ? 0.5f : 1;
	}

	private static Preferences instance = null;
	public static Preferences Singleton
	{
		get => instance;
	}
}