using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Project;

namespace BeatGame.scripts.preferences;

public enum SettingsKey
{
	CustomPlayerColor,
	ShowFps,
	CameraHeight,
	DisplayMode,
	VSync,
	Resolution,
	RenderScale,
	FpsLimit,
	AntialiasingLevel,
	MainVolume,
	MusicVolume,
	EffectsVolume,
}

public class SettingsContainer
{
	public string[] Tabs = Array.Empty<string>();
	public (int tabIndex, string name)[] Categories = Array.Empty<(int, string)>();
	public SettingsEntry[] Entries = Array.Empty<SettingsEntry>();
	public Dictionary<SettingsKey, SettingsEntry> EntryMap = new();

	public void AddEntry(SettingsEntry entry)
	{
		Entries = Entries.Append(entry).ToArray();
		EntryMap[entry.Key] = entry;
	}

	public ToggleSettingsEntry GetToggle(SettingsKey id)
	{
		return (ToggleSettingsEntry)EntryMap[id];
	}

	public SliderSettingsEntry GetSlider(SettingsKey id)
	{
		return (SliderSettingsEntry)EntryMap[id];
	}

	public DropdownSettingsEntry GetDropdown(SettingsKey id)
	{
		return (DropdownSettingsEntry)EntryMap[id];
	}

	public void SetValue(SettingsKey key, Variant value)
	{
		var target = EntryMap[key];
		switch (target)
		{
			case ToggleSettingsEntry toggle:
				toggle.SetValue(value.AsBool());
				break;
			case SliderSettingsEntry slider:
				slider.SetValue(value.AsSingle());
				break;
			case DropdownSettingsEntry dropdown:
				dropdown.SetValue(value.AsInt32());
				break;
			default:
				throw new ArgumentException($"Invalid value type for key {key} ({value.VariantType})");
		}
		target.UpdateUI();
	}

	public void CopyValuesFrom(SettingsContainer source)
	{
		foreach (var baseEntry in source.Entries)
		{
			switch (baseEntry)
			{
				case ToggleSettingsEntry toggle:
					SetValue(baseEntry.Key, toggle.Value);
					break;
				case SliderSettingsEntry slider:
					SetValue(baseEntry.Key, slider.Value);
					break;
				case DropdownSettingsEntry dropdown:
					SetValue(baseEntry.Key, dropdown.Selected.Value);
					break;
				default:
					throw new ArgumentException($"Invalid entry type {baseEntry.GetType()}");
			}
		}
	}

	public static SettingsContainer MakeDefault()
	{
		var defaultRenderScale = OS.HasFeature("macos") ? 0.5f : 1;

		return new SettingsContainerBuilder()
			.SetTab("Gameplay")
			// .AddSlider(SettingsKey.CustomPlayerColor, "Custom player color", min: 0, value: 1, max: 1, step: 1)
			// .AddSlider(SettingsKey.ShowFps, "Show FPS", min: 0, value: 1, max: 1, step: 1)
			.SetCategory("Camera Settings")
			.AddSlider(SettingsKey.CameraHeight, "Height", min: 0f, value: 0.5f, max: 1f, step: 0.05f, percentage: true)
			.SetTab("Video")
			.SetCategory("Basic Settings")
			.AddDropdown(SettingsKey.DisplayMode, "Display mode", new[]
			{
				("Borderless", 0),
				("Windowed", 1),
			}, 0)
			.AddToggle(SettingsKey.VSync, "VSync", value: true)
			.AddDropdown(SettingsKey.Resolution, "Resolution", new[]
			{
				("2560x1440", 0),
				("1920x1080", 1),
				("1440x900", 2),
				("1360x768", 3),
				("1280x720", 4),
				("1024x768", 5),
				("800x600", 6),
				("640x480", 7),
			}, 0)
			.AddSlider(SettingsKey.RenderScale, "Render scale", min: 0.25f, value: defaultRenderScale, max: 1f, step: 0.05f, percentage: true)
			.AddSlider(SettingsKey.FpsLimit, "Max FPS", min: 15, value: 165, max: 255, step: 15, textConverter: (v) => v > 240 ? "Inf" : v.ToString())
			.SetCategory("Advanced Settings")
			.AddDropdown(SettingsKey.AntialiasingLevel, "Anti Aliasing", new[]
			{
				("None", Antialiasing.None),
				("FXAA", Antialiasing.FXAA),
				("TAA", Antialiasing.TAA),
				("2x MSAA", Antialiasing.MSAA_2),
				("4x MSAA", Antialiasing.MSAA_4),
				("8x MSAA", Antialiasing.MSAA_8),
				("All of them", Antialiasing.ALL),
			}, Antialiasing.MSAA_2)
			.SetTab("Audio")
			.SetCategory("Volume Settings")
			.AddSlider(SettingsKey.MainVolume, "Main volume", min: 0f, value: 0.5f, max: 1f, step: 0.02f, percentage: true)
			.AddSlider(SettingsKey.MusicVolume, "Music volume", min: 0f, value: 0.5f, max: 1f, step: 0.02f, percentage: true)
			.AddSlider(SettingsKey.EffectsVolume, "Effects volume", min: 0f, value: 0.5f, max: 1f, step: 0.02f, percentage: true)
			.SetTab("Controls")
			.SetTab("Credit")
			.Build();
	}
}
