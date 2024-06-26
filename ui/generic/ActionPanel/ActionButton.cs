using Godot;
using System;
using System.Diagnostics;

namespace Project;

public partial class ActionButton : Control
{
	public string Label;
	public string ActionName;

	private TextureRect Icon;
	private TextureRect Overlay;
	private Label HotkeyLabel;
	private ShaderMaterial ButtonMaterial;
	private ProgressBar CooldownProgressBar;

	public bool IsHovered;
	public float HoveredValue;
	public bool IsPressed;
	public bool IsPressedWithKey;
	public float PressedValue;
	public bool IsDisabled;
	public float DisabledValue;
	public bool IsHighlighted;
	public float HighlightedValue;

	Timer tooltipDelayTimer;

	private BaseCast AssociatedCast;

	public override void _Ready()
	{
		Icon = GetNode<TextureRect>("Control/IconTextureRect");
		Overlay = GetNode<TextureRect>("Control/OverlayTextureRect");
		ButtonMaterial = (ShaderMaterial)Overlay.Material;
		HotkeyLabel = GetNode<Label>("Control/OverlayTextureRect/HotkeyLabel");
		CooldownProgressBar = GetNode<ProgressBar>("Control/CooldownProgressBar");
		HotkeyLabel.Text = Label.ToString();
		Overlay.MouseEntered += OnMouseEnter;
		Overlay.MouseExited += OnMouseLeave;

		SignalBus.Singleton.CastAssigned += OnCastAssigned;
		SignalBus.Singleton.CastUnassigned += OnCastUnassigned;
		Music.Singleton.BeatTick += OnBeatStateChanged;

		IsDisabled = true;
		Icon.Texture = GD.Load<CompressedTexture2D>("res://assets/ui/icon-skill-active-placeholder.png");

		tooltipDelayTimer = new Timer()
		{
			WaitTime = 0.75f,
			OneShot = true,
		};
		tooltipDelayTimer.Timeout += () =>
		{
			if (!IsHovered)
				return;

			SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CastHovered, AssociatedCast);
		};
		AddChild(tooltipDelayTimer);
	}

	private void OnMouseEnter()
	{
		if (IsDisabled)
			return;

		HoveredValue = 1;
		IsHovered = true;
		tooltipDelayTimer.Start();
	}

	private void OnMouseLeave()
	{
		IsHovered = false;
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CastUnhovered, AssociatedCast);
	}

	private void OnCastAssigned(BaseCast cast, string actionName)
	{
		if (actionName != ActionName)
			return;

		AssociatedCast = cast;
		IsDisabled = false;

		if (AssociatedCast.Settings.IconPath != null)
			Icon.Texture = GD.Load<CompressedTexture2D>(AssociatedCast.Settings.IconPath);
	}

	private void OnCastUnassigned(BaseCast cast, string actionName)
	{
		if (actionName != ActionName)
			return;

		AssociatedCast = null;
		IsDisabled = true;
		Icon.Texture = GD.Load<CompressedTexture2D>("res://assets/ui/icon-skill-active-placeholder.png");
	}

	private void OnBeatStateChanged(BeatTime time)
	{
		if (AssociatedCast == null || AssociatedCast.Settings.CastTimings.HasNot(time))
			return;

		HighlightedValue = 1;
	}

	public override void _Input(InputEvent @event)
	{
		if (SkillForestUI.Singleton.Visible || AssociatedCast == null)
			return;

		if (@event.IsActionPressed("MouseInteract") && IsHovered)
		{
			IsPressed = true;
			if (PlayerController.AllPlayers.Count > 0)
				PlayerController.AllPlayers[0].Spellcasting.CastInputPressed(AssociatedCast);
		}
		if (@event.IsActionReleased("MouseInteract") && IsPressed)
		{
			IsPressed = false;
			if (PlayerController.AllPlayers.Count > 0)
				PlayerController.AllPlayers[0].Spellcasting.CastInputReleased(AssociatedCast);
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

		if (IsHighlighted || (AssociatedCast != null && AssociatedCast.Settings.CastTimings == BeatTime.Free))
			HighlightedValue = Math.Min(1, HighlightedValue + (float)delta * 5);
		else
			HighlightedValue = Math.Max(0, HighlightedValue - (float)delta * 5);
		ButtonMaterial.SetShaderParameter("HighlightedValue", HighlightedValue);

		if (AssociatedCast == null)
		{
			CooldownProgressBar.Value = 0;
			return;
		}

		double value = 0;
		var currentCastingSpell = ((PlayerController)AssociatedCast.Parent).Spellcasting.GetCurrentCastingSpell();
		var cooldownTimeLeft = AssociatedCast.GetCooldownTimeLeft();
		if (cooldownTimeLeft > 0)
			value = cooldownTimeLeft / AssociatedCast.GetCooldownWaitTime();
		if (currentCastingSpell != null && currentCastingSpell.Settings.GlobalCooldown && AssociatedCast.Settings.GlobalCooldown
			&& (cooldownTimeLeft < BaseCast.GlobalCooldownDuration * Music.Singleton.SecondsPerBeat))
			value = 1;

		CooldownProgressBar.Value = value * 100;
	}
}
