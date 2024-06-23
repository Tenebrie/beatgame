using Godot;
using System;

namespace Project;

public partial class SkillPopup : BasePopup
{
	RichTextLabel skillNameLabel;
	RichTextLabel skillMetadataLabel;

	public override void _Ready()
	{
		base._Ready();

		Visible = false;
		FollowMouse();
		skillNameLabel = GetNode<RichTextLabel>("Label/SkillNameLabel");
		skillMetadataLabel = GetNode<RichTextLabel>("Label/VBoxContainer/SkillMetadataLabel");
		SignalBus.Singleton.SkillHovered += OnSkillHovered;
		SignalBus.Singleton.SkillUnhovered += OnSkillUnhovered;
	}

	public void SetSkillName(string text)
	{
		skillNameLabel.Text = text;
		RecalculateOffsets();
	}

	public void SetSkillMetadata(string text)
	{
		skillMetadataLabel.Text = text;
	}

	public override void SetBody(string text)
	{
		base.SetBody("\n\n" + text);
		RecalculateOffsets();
	}

	void OnSkillHovered(BaseSkill skill)
	{
		MakeVisible();
		SetSkillName(skill.Settings.FriendlyName);
		SetBody(skill.Description);
		if (skill.Settings.ActiveCast != null)
			SetSkillMetadata(skill.Settings.ActiveCast.Settings.GetReadableCastTimings());
		else
			SetSkillMetadata("");

		var mousePos = GetViewport().GetMousePosition();
		var screenSize = GetViewport().GetVisibleRect().Size;
		if (mousePos.X < screenSize.X / 2)
			SetHorizontalAnchor(Anchor.Begin);
		else
			SetHorizontalAnchor(Anchor.End);

		if (mousePos.Y < screenSize.Y / 2)
			SetVerticalAnchor(Anchor.Begin);
		else
			SetVerticalAnchor(Anchor.End);
	}

	void OnSkillUnhovered(BaseSkill skill)
	{
		MakeHidden();
	}
}