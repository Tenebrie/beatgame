using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
			if (AssociatedSkill.Settings.IconPath != null)
				Icon.Texture = GD.Load<CompressedTexture2D>(AssociatedSkill.Settings.IconPath);
			CheckForIncompatibles();
		}
	}

	void OnSkillUp(BaseSkill skill)
	{
		if (skill == AssociatedSkill)
			IsSelected = true;

		CheckForIncompatibles();
	}

	void OnSkillDown(BaseSkill skill)
	{
		if (skill == AssociatedSkill)
			IsSelected = false;

		CheckForIncompatibles();
	}

	void CheckForIncompatibles()
	{
		IsDisabled = AssociatedSkill.Settings.Disabled ||
			SkillTreeManager.Singleton.Skills
				.Where(skill => skill.IsLearned)
				.Any(s => s.Settings.IncompatibleSkills.Any(wrapper => wrapper.Is(AssociatedSkill)));
	}

	private void OnMouseEnter()
	{
		HoveredValue = 1;
		IsHovered = true;
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.SkillHovered, AssociatedSkill);
	}

	private void OnMouseLeave()
	{
		IsHovered = false;
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.SkillUnhovered, AssociatedSkill);
	}

	public void AssignSkill(BaseSkill skill)
	{
		AssociatedSkill = skill;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("MouseInteract") && IsHovered && !IsDisabled)
		{
			IsPressed = true;
		}
		if (@event.IsActionReleased("MouseInteract") && IsPressed)
		{
			IsPressed = false;
			if (IsHovered)
				OnInteract();
		}
		if (@event.IsActionPressed("MouseInteractAlt") && IsHovered && !IsDisabled)
		{
			IsPressedAlt = true;
		}
		if (@event.IsActionReleased("MouseInteractAlt") && IsPressedAlt)
		{
			IsPressedAlt = false;
			if (IsHovered)
				OnInteractAlt();
		}

		if (!IsHovered || PlayerController.AllPlayers.Count == 0 || AssociatedSkill.Settings.ActiveCast == null)
			return;

		List<string> CastActions = PlayerSpellcasting.GetPossibleBindings();

		var player = PlayerController.AllPlayers[0];
		foreach (var action in CastActions)
		{
			if (@event.IsActionPressed(action, exactMatch: true))
			{
				player.Spellcasting.UnbindAll(AssociatedSkill.Settings.ActiveCast.CastType);
				player.Spellcasting.Bind(action, AssociatedSkill.Settings.ActiveCast.Create(player));
			}
		}
	}

	void OnInteract()
	{
		if (!AssociatedSkill.IsLearned)
			SkillTreeManager.Singleton.LearnSkill(AssociatedSkill);
		else if (AssociatedSkill.Settings.ActiveCast != null)
		{
			var msg = Lib.LoadScene(Lib.UI.SkillBindingNotification).Instantiate<Control>();
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
