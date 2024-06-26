using Godot;
using System;
using System.Diagnostics;

namespace Project;

public partial class BuffButton : Control
{
	[Export] TextureRect Icon;
	[Export] Label StacksLabel;
	private ShaderMaterial ButtonMaterial;

	public bool IsHovered;
	public float HoveredValue;
	public bool IsPressed;
	public float PressedValue;

	public BaseBuff AssociatedBuff;

	public override void _Ready()
	{
		Icon = GetNode<TextureRect>("Control/IconTextureRect");
		ButtonMaterial = (ShaderMaterial)Icon.Material;
		Icon.MouseEntered += OnMouseEnter;
		Icon.MouseExited += OnMouseLeave;

		Icon.Texture = GD.Load<CompressedTexture2D>("res://assets/ui/icon-skill-active-placeholder.png");
	}

	private void OnMouseEnter()
	{
		HoveredValue = 1;
		IsHovered = true;
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.BuffHovered, AssociatedBuff);
	}

	private void OnMouseLeave()
	{
		IsHovered = false;
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.BuffUnhovered, AssociatedBuff);
	}

	public void AssignBuff(BaseBuff buff)
	{
		AssociatedBuff = buff;
		UpdateStacks();

		if (AssociatedBuff.Settings.IconPath != null)
			Icon.Texture = GD.Load<CompressedTexture2D>(AssociatedBuff.Settings.IconPath);
	}

	public int UpdateStacks()
	{
		var stacks = AssociatedBuff.Parent.Buffs.Stacks(AssociatedBuff.GetType());
		if (stacks == 1)
			StacksLabel.Hide();
		else
		{
			StacksLabel.Show();
			StacksLabel.Text = stacks.ToString();
		}
		return stacks;
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
		var time = CastUtils.GetTicksSec();
		if (AssociatedBuff == null)
			return;

		var progress = (time - AssociatedBuff.CreatedAt) / (AssociatedBuff.ExpiresAt - AssociatedBuff.CreatedAt);
		ButtonMaterial.SetShaderParameter("Progress", progress);

		if (IsHovered || IsPressed)
			HoveredValue = Math.Min(1, HoveredValue + (float)delta * 5);
		else
			HoveredValue = Math.Max(0, HoveredValue - (float)delta * 5);
		ButtonMaterial.SetShaderParameter("HoveredValue", HoveredValue);

		if (IsPressed)
			PressedValue = Math.Min(1, PressedValue + (float)delta * 5);
		else
			PressedValue = Math.Max(0, PressedValue - (float)delta * 5);
		ButtonMaterial.SetShaderParameter("PressedValue", PressedValue);
	}
}
