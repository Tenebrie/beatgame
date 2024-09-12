using System;
using Godot;

namespace BeatGame.scripts.preferences;

public static class PreferencesFilesystem
{
	public static bool LoadConfig(SettingsContainer loadInto)
	{
		var config = new ConfigFile();
		config.Load("user://config.cfg");
		try
		{
			TryLoadConfig(config, loadInto);
			return true;
		}
		catch (Exception ex)
		{
			GD.PrintErr("Failed to load config file: " + ex.Message);
			return false;
		}
	}

	public static void TryLoadConfig(ConfigFile config, SettingsContainer loadInto)
	{
		foreach (SettingsKey settingsKey in (SettingsKey[])Enum.GetValues(typeof(SettingsKey)))
		{
			var key = settingsKey.ToString();
			if (!config.HasSectionKey("userPreferences", key))
				continue;

			var value = config.GetValue("userPreferences", key);
			loadInto.SetValue(settingsKey, value);
		}
	}

	public static bool SaveConfig(SettingsContainer saveFrom)
	{
		var config = new ConfigFile();

		foreach (var entry in saveFrom.Entries)
		{
			GD.Print("Saving entry: " + entry.Key + " = " + entry);
			var key = entry.Key.ToString();
			if (entry is SliderSettingsEntry slider)
				config.SetValue("userPreferences", key, slider.Value);
			else if (entry is DropdownSettingsEntry dropdown)
				config.SetValue("userPreferences", key, dropdown.Selected.Value);
		}

		config.Save("user://config.cfg");
		return true;
	}
}