using Godot;
using System;

namespace Project;

public partial class SettingsDirtyModalUI : Control
{
	[Export] public Button ApplyButton;
	[Export] public Button DiscardButton;
	[Export] public Button CancelButton;

	public override void _Ready()
	{
		Visible = false;

		ApplyButton.Pressed += () => Visible = false;
		DiscardButton.Pressed += () => Visible = false;
		CancelButton.Pressed += () => Visible = false;
	}

	public bool HandleEscapeKey()
	{
		Visible = false;
		return true;
	}
}
