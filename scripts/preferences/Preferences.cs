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
	public FogQuality FogQualityLevel => (FogQuality)DraftSettings.GetDropdown(SettingsKey.FogQuality).Selected.Value;

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
		ApplyPreferences(applyAll: true);
		EnableLivePreview();
	}

	void EnableLivePreview()
	{
		foreach (var entry in DraftSettings.Entries)
		{
			if (entry.OnApplySideEffects is not null)
				continue;

			entry.OnApplySideEffects += () => TemporarilyApplyDraftSettings(entry.Key);
		}
	}

	public void ApplyPreferences(bool applyAll, params SettingsKey[] keys)
	{
		if (applyAll || keys.Has(SettingsKey.MainVolume, SettingsKey.MusicVolume, SettingsKey.EffectsVolume))
			ApplyAudioVolume();
		if (applyAll || keys.Has(SettingsKey.RenderScale))
			ApplyRenderScale();
		if (applyAll || keys.Has(SettingsKey.FpsLimit))
			ApplyFramerateLimit();
		if (applyAll || keys.Has(SettingsKey.AmbientOcclusion))
			ApplyAmbientOcclusion();
		if (applyAll || keys.Has(SettingsKey.VSync))
			ApplyVSync();
		if (applyAll || keys.Has(SettingsKey.FogQuality))
			ApplyFogQuality();
		if (applyAll || keys.Has(SettingsKey.AntialiasingLevel))
			ApplyAntialiasing();
	}

	public void TemporarilyApplyDraftSettings(params SettingsKey[] keys)
	{
		EmitSignal(SignalName.DraftChanged);
		AppliedSettings.CopyValuesFrom(DraftSettings);
		ApplyPreferences(applyAll: false, keys);
		AppliedSettings.CopyValuesFrom(SavedSettings);
	}

	public void ApplyAndSaveDraftSettings()
	{
		AppliedSettings.CopyValuesFrom(DraftSettings);
		ApplyPreferences(applyAll: true);
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