using Godot;
using System;

namespace Project;

public partial class CastPopup : BasePopup
{
	[Export] RichTextLabel castNameLabel;
	[Export] RichTextLabel castMetadataLabel;

	public override void _Ready()
	{
		base._Ready();

		Visible = false;
		FollowMouse();
		SignalBus.Singleton.CastHovered += OnCastHovered;
		SignalBus.Singleton.CastUnhovered += OnCastUnhovered;
	}

	public void SetCastName(string text)
	{
		castNameLabel.Text = text;
		RecalculateOffsets();
	}

	public void SetCastMetadata(string text)
	{
		castMetadataLabel.Text = text;
	}

	public override void SetBody(string text)
	{
		base.SetBody("\n\n" + text);
		RecalculateOffsets();
	}

	void OnCastHovered(BaseCast cast)
	{
		MakeVisible();
		SetCastName(cast.Settings.FriendlyName);
		SetBody(BaseCast.GetDescription(cast.Settings));
		SetCastMetadata(cast.Settings.GetReadableCastTimings());
	}

	void OnCastUnhovered(BaseCast cast)
	{
		MakeHidden();
	}
}
