using Godot;
using System;
using System.Diagnostics;

namespace Project;

public partial class ActionButton : Control
{
	public string Label;
	public StringName ActionName;

	private TextureRect Icon;
	private TextureRect Overlay;
	private TextureRect QueuedIcon;
	private TextureRect AutoAttackIcon;
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
		QueuedIcon = GetNode<TextureRect>("Control/OverlayTextureRect/QueuedIconTextureRect");
		AutoAttackIcon = GetNode<TextureRect>("Control/OverlayTextureRect/AAIconTextureRect");
		ButtonMaterial = (ShaderMaterial)Overlay.Material;
		HotkeyLabel = GetNode<Label>("Control/OverlayTextureRect/HotkeyLabel");
		CooldownProgressBar = GetNode<ProgressBar>("Control/CooldownProgressBar");
		HotkeyLabel.Text = Label;
		Overlay.MouseEntered += OnMouseEnter;
		Overlay.MouseExited += OnMouseLeave;

		SignalBus.Singleton.CastAssigned += OnCastAssigned;
		SignalBus.Singleton.CastUnassigned += OnCastUnassigned;
		SignalBus.Singleton.CastAssignedAsAuto += OnCastAssignedAsAuto;
		SignalBus.Singleton.CastUnassignedAsAuto += OnCastUnassignedAsAuto;
		SignalBus.Singleton.CastQueued += OnCastQueued;
		SignalBus.Singleton.CastUnqueued += OnCastUnqueued;
		Music.Singleton.BeatTick += OnBeatStateChanged;

		IsDisabled = true;
		Icon.Texture = GD.Load<CompressedTexture2D>("res://assets/ui/icon-skill-active-placeholder.png");
		QueuedIcon.Visible = false;
		AutoAttackIcon.Visible = false;

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

	private void OnCastAssigned(BaseCast cast, StringName actionName)
	{
		if (actionName != ActionName)
			return;

		AssociatedCast = cast;
		IsDisabled = false;

		if (AssociatedCast.Settings.IconPath != null)
			Icon.Texture = GD.Load<CompressedTexture2D>(AssociatedCast.Settings.IconPath);
		AutoAttackIcon.Visible = cast.AutoAttack.IsAutoCasting;
	}

	private void OnCastUnassigned(BaseCast cast, StringName actionName)
	{
		if (actionName != ActionName)
			return;

		AssociatedCast = null;
		IsDisabled = true;
		Icon.Texture = GD.Load<CompressedTexture2D>("res://assets/ui/icon-skill-active-placeholder.png");
	}

	private void OnCastAssignedAsAuto(BaseCast cast)
	{
		AutoAttackIcon.Visible = cast == AssociatedCast;
	}

	private void OnCastUnassignedAsAuto(BaseCast cast)
	{
		AutoAttackIcon.Visible = false;
	}

	private void OnCastQueued(BaseCast cast)
	{
		QueuedIcon.Visible = cast == AssociatedCast;
	}

	private void OnCastUnqueued(BaseCast cast)
	{
		QueuedIcon.Visible = false;
	}

	private void OnBeatStateChanged(BeatTime time)
	{
		if (AssociatedCast == null || time.HasNot(BeatTime.Whole | BeatTime.Half | BeatTime.Quarter))
			return;

		HighlightedValue = 1;
	}

	public override void _Input(InputEvent @event)
	{
		if (SkillForestUI.Singleton.Visible || AssociatedCast == null)
			return;

		if (@event.IsActionPressed("MouseInteract".ToStringName()) && IsHovered)
		{
			IsPressed = true;
			if (PlayerController.AllPlayers.Count > 0)
				PlayerController.AllPlayers[0].Spellcasting.CastInputPressed(AssociatedCast);
		}
		if (@event.IsActionReleased("MouseInteract".ToStringName()) && IsPressed)
		{
			IsPressed = false;
		}

		if (@event.IsActionPressed("MouseInteractAlt".ToStringName()) && IsHovered && AssociatedCast != null)
		{
			AssociatedCast.AutoAttack.Toggle();
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
		ButtonMaterial.SetShaderParameter("HoveredValue".ToStringName(), HoveredValue);

		if (IsPressed)
			PressedValue = Math.Min(1, PressedValue + (float)delta * 5);
		else
			PressedValue = Math.Max(0, PressedValue - (float)delta * 5);
		ButtonMaterial.SetShaderParameter("PressedValue".ToStringName(), PressedValue);

		if (IsDisabled)
			DisabledValue = Math.Min(1, DisabledValue + (float)delta * 5);
		else
			DisabledValue = 0;
		ButtonMaterial.SetShaderParameter("DisabledValue".ToStringName(), DisabledValue);

		if (IsHighlighted)
			HighlightedValue = Math.Min(1, HighlightedValue + (float)delta * 5);
		else
			HighlightedValue = Math.Max(0, HighlightedValue - (float)delta * 5);
		ButtonMaterial.SetShaderParameter("HighlightedValue".ToStringName(), HighlightedValue);

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
		// If the currently casting cast will trigger a GCD that the associated cast will receive - darken the icon in advance
		if (currentCastingSpell != null
			&& currentCastingSpell.Settings.GlobalCooldown.Triggers()
			&& AssociatedCast.Settings.GlobalCooldown.Receives()
			&& (cooldownTimeLeft < BaseCast.GlobalCooldownDuration * Music.Singleton.SecondsPerBeat))
			value = 1;

		CooldownProgressBar.Value = value * 100;
	}
}
