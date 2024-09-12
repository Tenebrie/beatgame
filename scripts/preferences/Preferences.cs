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

	public bool ShowFps => AppliedSettings.GetSlider(SettingsKey.ShowFps).Value == 1;
	public float MainVolume => AppliedSettings.GetSlider(SettingsKey.MainVolume).Value;
	public float MusicVolume => AppliedSettings.GetSlider(SettingsKey.MusicVolume).Value;
	public float AudioVolume => AppliedSettings.GetSlider(SettingsKey.EffectsVolume).Value;
	public float CameraHeight => AppliedSettings.GetSlider(SettingsKey.CameraHeight).Value;
	public float RenderScale => AppliedSettings.GetSlider(SettingsKey.RenderScale).Value;
	public Antialiasing AntialiasingLevel => (Antialiasing)AppliedSettings.GetDropdown(SettingsKey.AntialiasingLevel).Selected.Value;

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