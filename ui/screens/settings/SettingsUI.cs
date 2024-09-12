using BeatGame.scripts.preferences;
using Godot;

namespace Project;

public partial class SettingsUI : Control
{
	[Export] Control TabPanelContainer;
	[Export] Button ApplyButton;
	[Export] Button DiscardButton;
	[Export] Button RestoreDefaultsButton;
	[Export] Button CloseButton;
	[Export] public SettingsDirtyModalUI DirtyModal;

	bool IsDirty = false;

	public override void _Ready()
	{
		instance = this;
		Visible = false;

		InitUI(Preferences.Singleton.DraftSettings);
		SetClean();
		ApplyButton.Pressed += () =>
		{
			Preferences.Singleton.ApplyAndSaveDraftSettings();
			SetClean();
		};
		DiscardButton.Pressed += () =>
		{
			Preferences.Singleton.DiscardDraftSettings();
			SetClean();
		};
		RestoreDefaultsButton.Pressed += () =>
		{
			Preferences.Singleton.RestoreDefaultSettings();
			SetDirty();
		};
		CloseButton.Pressed += () => AttemptClose();

		DirtyModal.ApplyButton.Pressed += () =>
		{
			Preferences.Singleton.ApplyAndSaveDraftSettings();
			SetClean();
			Visible = false;
		};

		DirtyModal.DiscardButton.Pressed += () =>
		{
			Preferences.Singleton.DiscardDraftSettings();
			SetClean();
			Visible = false;
		};

		Preferences.Singleton.DraftChanged += SetDirty;
	}

	void SetDirty()
	{
		IsDirty = true;
		ApplyButton.Disabled = false;
		DiscardButton.Disabled = false;
	}

	void SetClean()
	{
		IsDirty = false;
		ApplyButton.Disabled = true;
		DiscardButton.Disabled = true;
	}

	public bool HandleEscapeKey() => AttemptClose();

	public bool AttemptClose()
	{
		if (IsDirty)
		{
			DirtyModal.Visible = true;
		}
		else
		{
			Visible = false;
		}
		return true;
	}

	static SettingsUI instance;
	public static SettingsUI Singleton => instance;
}
