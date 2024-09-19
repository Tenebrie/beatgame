using System;
using BeatGame.scripts.preferences;
using Godot;
using Project;

namespace BeatGame.scripts.preferences;

public partial class Preferences : Node
{
	[Signal] public delegate void DraftChangedEventHandler();

	public SettingsContainer DraftSettings;
	public SettingsContainer AppliedSettings;
	public SettingsContainer SavedSettings;

	public float CameraHeight => DraftSettings.GetSlider(SettingsKey.CameraHeight).Value;

	public DisplayServer.VSyncMode VSyncMode => DraftSettings.GetToggle(SettingsKey.VSync).Value ? DisplayServer.VSyncMode.Enabled : DisplayServer.VSyncMode.Disabled;
	public float RenderScale => DraftSettings.GetSlider(SettingsKey.RenderScale).Value;
	public int FpsLimit => (int)Math.Round(DraftSettings.GetSlider(SettingsKey.FpsLimit).Value);
	public bool ShowFps => DraftSettings.GetSlider(SettingsKey.ShowFps).Value == 1;

	public Antialiasing AntialiasingLevel => (Antialiasing)DraftSettings.GetDropdown(SettingsKey.AntialiasingLevel).Selected.Value;
	public bool AmbientOcclusion => DraftSettings.GetToggle(SettingsKey.AmbientOcclusion).Value;

	public float MainVolume => DraftSettings.GetSlider(SettingsKey.MainVolume).Value;
	public float MusicVolume => DraftSettings.GetSlider(SettingsKey.MusicVolume).Value;
	public float AudioVolume => DraftSettings.GetSlider(SettingsKey.EffectsVolume).Value;

	public override void _EnterTree()
	{
		instance = this;
		DraftSettings = SettingsContainer.MakeDefault();
		AppliedSettings = SettingsContainer.MakeDefault();
		SavedSettings = SettingsContainer.MakeDefault();
		if (PreferencesFilesystem.LoadConfig(SavedSettings))
		{
			DraftSettings.CopyValuesFrom(SavedSettings);
			AppliedSettings.CopyValuesFrom(SavedSettings);
		}
		ApplyPreferences();
		EnableLivePreview();
	}

	void EnableLivePreview()
	{
		foreach (var entry in DraftSettings.Entries)
		{
			entry.OnApplySideEffects += TemporarilyApplyDraftSettings;
		}
	}

	public void ApplyPreferences()
	{
		AudioServer.SetBusVolumeDb(0, Mathf.LinearToDb(MainVolume));
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Music"), Mathf.LinearToDb(MusicVolume));
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Effects"), Mathf.LinearToDb(AudioVolume));

		GetViewport().Scaling3DScale = RenderScale;
		Engine.Singleton.MaxFps = FpsLimit <= 240 ? FpsLimit : 1000;
		DisplayServer.Singleton.WindowSetVsyncMode(VSyncMode);
		var environment = EnvironmentController.Singleton;
		if (environment != null && environment.WorldEnvironment != null)
		{
			environment.WorldEnvironment.Environment.SsaoEnabled = AmbientOcclusion;
		}
		ApplyAntialiasing();
	}

	public void TemporarilyApplyDraftSettings()
	{
		EmitSignal(SignalName.DraftChanged);
		AppliedSettings.CopyValuesFrom(DraftSettings);
		ApplyPreferences();
		AppliedSettings.CopyValuesFrom(SavedSettings);
	}

	public void ApplyAndSaveDraftSettings()
	{
		AppliedSettings.CopyValuesFrom(DraftSettings);
		ApplyPreferences();
		PreferencesFilesystem.SaveConfig(AppliedSettings);
		SavedSettings.CopyValuesFrom(AppliedSettings);
	}

	public void DiscardDraftSettings()
	{
		DraftSettings.CopyValuesFrom(SavedSettings);
	}

	public void RestoreDefaultSettings()
	{
		DraftSettings.CopyValuesFrom(SettingsContainer.MakeDefault());
	}

	private static Preferences instance = null;
	public static Preferences Singleton => instance;
}