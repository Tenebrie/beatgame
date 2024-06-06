using Godot;
using System;
using System.Diagnostics;

namespace Project;

public partial class ActionButton : Control
{
	public string Label;
	public string ActionName;

	private TextureRect Button;
	private Label HotkeyLabel;
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

	private BaseCast AssociatedCast;

	public override void _Ready()
	{
		Button = GetNode<TextureRect>("Control/TextureRect");
		ButtonMaterial = (ShaderMaterial)Button.Material;
		HotkeyLabel = GetNode<Label>("Control/TextureRect/HotkeyLabel");
		HotkeyLabel.Text = Label.ToString();
		Button.MouseEntered += OnMouseEnter;
		Button.MouseExited += OnMouseLeave;

		SignalBus.Singleton.CastAssigned += OnCastAssigned;
		Music.Singleton.BeatTick += OnBeatStateChanged;
		Music.Singleton.BeatWindowLock += OnBeatStateChanged;

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

	private void OnCastAssigned(BaseCast cast, string actionName)
	{
		if (actionName != ActionName)
			return;

		AssociatedCast = cast;
		IsDisabled = false;	
	}

	private void OnBeatStateChanged(BeatTime time)
	{
		if (AssociatedCast == null)
			return;

		IsHighlighted = AssociatedCast.ValidateCastTiming(out _);
		if (IsHighlighted)
			HighlightedValue = 1;
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

		if (@event.IsActionPressed(ActionName, exactMatch: true))
		{
			IsPressedWithKey = true;
			HoveredValue = 1;
		}
		if (@event.IsActionReleased(ActionName, exactMatch: true))
		{
			IsPressedWithKey = false;
			if (!IsHovered && !IsPressed)
				HoveredValue = 0;
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

		if (AssociatedCast == null)
			return;

		if (IsHighlighted)
			HighlightedValue = Math.Min(1, HighlightedValue + (float)delta * 5);
		else if (AssociatedCast.Settings.CastTimings != BeatTime.Free)
			HighlightedValue = Math.Max(0, HighlightedValue - (float)delta * 5);
		ButtonMaterial.SetShaderParameter("HighlightedValue", HighlightedValue);
	}
}
