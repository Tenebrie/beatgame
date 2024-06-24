using Godot;
using System;
using System.Text;

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

		StringBuilder builder = new();
		if (buff.Settings.Description != null)
			builder.Append(buff.Settings.Description);
		if (buff.Settings.Description != null && buff.Settings.DynamicDesc != null)
			builder.Append('\n');
		if (buff.Settings.DynamicDesc != null)
			builder.Append(buff.Settings.DynamicDesc());
		SetBody(builder.ToString());
	}

	void OnBuffUnhovered(BaseBuff buff)
	{
		MakeHidden();
	}
}
