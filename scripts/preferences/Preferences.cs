using Godot;

namespace Project;

public partial class Preferences : Node
{
	public float mainVolume = 0.5f;
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

	public override void _EnterTree()
	{
		instance = this;

		LoadConfig();
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
		var err = config.Load("user://config.cfg");

		// if (err != Error.Ok)
		// 	return;

		mainVolume = (float)config.GetValue("section", "mainVolume", 0.5f);
	}

	private static Preferences instance = null;
	public static Preferences Singleton
	{
		get => instance;
	}
}