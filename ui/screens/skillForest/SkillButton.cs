using Godot;
using System;
using System.Diagnostics;

namespace Project;

public partial class SkillButton : Control
{
	public string Label;
	public string ActionName;

	private TextureRect Button;
	private ShaderMaterial ButtonMaterial;

	public bool IsHovered;
	public float HoveredValue;
	public bool IsPressed;
	public bool IsPressedWithKey;
	public float PressedValue;
	public bool IsDisabled;
	public float DisabledValue;
	public bool IsHighlighted;
	public float HighlightedValue;

	private BaseSkill AssociatedSkill;

	public override void _Ready()
	{
		Button = GetNode<TextureRect>("Control/TextureRect");
		ButtonMaterial = (ShaderMaterial)Button.Material;
		Button.MouseEntered += OnMouseEnter;
		Button.MouseExited += OnMouseLeave;

		IsDisabled = true;
	}

	private void OnMouseEnter()
	{
		if (IsDisabled)
			return;

		HoveredValue = 1;
		IsHovered = true;
	}

	private void OnMouseLeave()
	{
		IsHovered = false;
	}

	public void AssignSkill(BaseSkill skill)
	{
		AssociatedSkill = skill;
		IsDisabled = false;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("MouseInteract") && IsHovered)
		{
			IsPressed = true;
		}
		if (@event.IsActionReleased("MouseInteract") && IsPressed)
		{
			IsPressed = false;
		}
	}

	public override void _Process(double delta)
	{
		if (IsHovered || IsPressed || IsPressedWithKey)
			HoveredValue = Math.Min(1, HoveredValue + (float)delta * 5);
		else
			HoveredValue = Math.Max(0, HoveredValue - (float)delta * 5);
		ButtonMaterial.SetShaderParameter("HoveredValue", HoveredValue);

		if (IsPressed)
			PressedValue = Math.Min(1, PressedValue + (float)delta * 5);
		else
			PressedValue = Math.Max(0, PressedValue - (float)delta * 5);
		ButtonMaterial.SetShaderParameter("PressedValue", PressedValue);

		if (IsDisabled)
			DisabledValue = Math.Min(1, DisabledValue + (float)delta * 5);
		else
			DisabledValue = 0;
		ButtonMaterial.SetShaderParameter("DisabledValue", DisabledValue);

		if (AssociatedSkill == null)
			return;

		if (IsHighlighted)
			HighlightedValue = Math.Min(1, HighlightedValue + (float)delta * 5);
		else
			HighlightedValue = Math.Max(0, HighlightedValue - (float)delta * 5);
		ButtonMaterial.SetShaderParameter("HighlightedValue", HighlightedValue);
	}
}
