using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Project;

public partial class SkillButton : Control
{
	public string Label;
	public string ActionName;

	private TextureRect Icon;
	private TextureRect Overlay;
	private ShaderMaterial IconMaterial;
	private ShaderMaterial OverlayMaterial;

	public bool IsHovered;
	public float HoveredValue;
	public bool IsPressed;
	public bool IsPressedAlt;
	public float PressedValue;
	public bool IsDisabled;
	public float DisabledValue;
	public bool IsHighlighted;
	public float HighlightedValue;
	public bool IsSelected;
	public float SelectedValue;

	private BaseSkill AssociatedSkill;

	public override void _Ready()
	{
		Icon = GetNode<TextureRect>("Control/IconTextureRect");
		Overlay = GetNode<TextureRect>("Control/OverlayTextureRect");
		IconMaterial = (ShaderMaterial)Icon.Material;
		OverlayMaterial = (ShaderMaterial)Overlay.Material;
		Overlay.MouseEntered += OnMouseEnter;
		Overlay.MouseExited += OnMouseLeave;
		SkillTreeManager.Singleton.SkillUp += OnSkillUp;
		SkillTreeManager.Singleton.SkillDown += OnSkillDown;

		IsDisabled = true;
		if (AssociatedSkill != null)
		{
			IsDisabled = false;
			if (AssociatedSkill.IconPath != null)
			{
				Icon.Texture = GD.Load<CompressedTexture2D>(AssociatedSkill.IconPath);
			}
		}
	}

	void OnSkillUp(BaseSkill skill)
	{
		if (skill == AssociatedSkill)
			IsSelected = true;
	}

	void OnSkillDown(BaseSkill skill)
	{
		if (skill == AssociatedSkill)
			IsSelected = false;
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
			if (IsHovered)
				OnInteract();
		}
		if (@event.IsActionPressed("MouseInteractAlt") && IsHovered)
		{
			IsPressedAlt = true;
		}
		if (@event.IsActionReleased("MouseInteractAlt") && IsPressedAlt)
		{
			IsPressedAlt = false;
			if (IsHovered)
				OnInteractAlt();
		}

		if (!IsHovered || PlayerController.AllPlayers.Count == 0 || AssociatedSkill.ActiveCast == null)
			return;

		List<string> CastActions = new()
		{
			"Cast1",
			"Cast2",
			"Cast3",
			"Cast4",
			"ShiftCast1",
			"ShiftCast2",
			"ShiftCast3",
			"ShiftCast4",
		};

		var player = PlayerController.AllPlayers[0];
		foreach (var action in CastActions)
		{
			if (@event.IsActionPressed(action, exactMatch: true))
			{
				player.Spellcasting.Bind(action, AssociatedSkill.ActiveCast.Create(player));
			}
		}
	}

	void OnInteract()
	{
		if (!AssociatedSkill.IsLearned)
			SkillTreeManager.Singleton.LearnSkill(AssociatedSkill);
		else
		{
			var msg = Lib.Scene(Lib.UI.SkillBindingNotification).Instantiate<Control>();
			msg.Position = GetViewport().GetMousePosition() + new Vector2(0, -50);
			GetTree().CurrentScene.AddChild(msg);
			SignalBus.SendMessage("Press the hotkey while hovering over the skill to assign it to the action bar.");
		}
	}

	void OnInteractAlt()
	{
		SkillTreeManager.Singleton.UnlearnSkill(AssociatedSkill);
	}

	public override void _Process(double delta)
	{
		if (IsHovered || IsPressed || IsPressedAlt)
			HoveredValue = Math.Min(1, HoveredValue + (float)delta * 5);
		else
			HoveredValue = Math.Max(0, HoveredValue - (float)delta * 5);
		OverlayMaterial.SetShaderParameter("HoveredValue", HoveredValue);

		if (IsPressed || IsPressedAlt)
			PressedValue = Math.Min(1, PressedValue + (float)delta * 5);
		else
			PressedValue = Math.Max(0, PressedValue - (float)delta * 5);
		OverlayMaterial.SetShaderParameter("PressedValue", PressedValue);

		if (IsDisabled)
			DisabledValue = Math.Min(1, DisabledValue + (float)delta * 5);
		else
			DisabledValue = 0;
		OverlayMaterial.SetShaderParameter("DisabledValue", DisabledValue);

		if (IsSelected)
			SelectedValue = Math.Min(1, SelectedValue + (float)delta * 5);
		else
			SelectedValue = Math.Max(0, SelectedValue - (float)delta * 5);
		OverlayMaterial.SetShaderParameter("SelectedValue", SelectedValue);

		if (IsHighlighted)
			HighlightedValue = Math.Min(1, HighlightedValue + (float)delta * 5);
		else
			HighlightedValue = Math.Max(0, HighlightedValue - (float)delta * 5);
		OverlayMaterial.SetShaderParameter("HighlightedValue", HighlightedValue);
	}
}
