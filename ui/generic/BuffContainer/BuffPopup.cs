using Godot;
using System;

namespace Project;

public partial class BuffPopup : BasePopup
{
	[Export]
	RichTextLabel buffNameLabel;

	public override void _Ready()
	{
		base._Ready();

		Visible = false;
		FollowMouse();
		SignalBus.Singleton.BuffHovered += OnBuffHovered;
		SignalBus.Singleton.BuffUnhovered += OnBuffUnhovered;
	}

	public void SetBuffName(string text)
	{
		buffNameLabel.Text = text;
		RecalculateOffsets();
	}

	public override void SetBody(string text)
	{
		base.SetBody("\n\n" + text);
		RecalculateOffsets();
	}

	void OnBuffHovered(BaseBuff buff)
	{
		MakeVisible();
		SetBuffName(buff.Settings.FriendlyName);
		SetBody(buff.Settings.Description);
	}

	void OnBuffUnhovered(BaseBuff buff)
	{
		MakeHidden();
	}
}
