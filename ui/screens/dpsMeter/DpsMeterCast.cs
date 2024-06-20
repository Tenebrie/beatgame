using Godot;
using System;

namespace Project;

public partial class DpsMeterCast : Control
{
	public float DamageDealt;
	public float FractionOfHighestDamage;

	TextureRect castIconRect;
	ProgressBar progressBar;
	Label castNameLabel;
	Label dpsPercentageLabel;
	public override void _Ready()
	{
		castIconRect = GetNode<TextureRect>("Panel/MarginContainer/Control/CastIcon");
		progressBar = GetNode<ProgressBar>("Panel/MarginContainer/Control/ProgressBar");
		castNameLabel = GetNode<Label>("Panel/MarginContainer/Control/CastNameLabel");
		dpsPercentageLabel = GetNode<Label>("Panel/MarginContainer/Control/DpsPercentageLabel");
	}

	public void SetCast(BaseCast cast)
	{
		castIconRect.Texture = GD.Load<CompressedTexture2D>(cast.Settings.IconPath);
		if (cast.Parent is PlayerController)
			castNameLabel.Text = cast.Settings.FriendlyName;
		else
			castNameLabel.Text = cast.Parent.FriendlyName + ": " + cast.Settings.FriendlyName;
	}

	public void SetFractionOfHighestDamageCast(float value)
	{
		progressBar.Value = value;
		FractionOfHighestDamage = value;
	}

	public void SetFractionOfTotalDamage(float value)
	{
		dpsPercentageLabel.Text = Math.Round(value * 100) + "%";
	}
}
