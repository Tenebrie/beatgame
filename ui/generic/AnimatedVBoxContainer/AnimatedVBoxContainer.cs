using Godot;
using System;
using System.Collections.Generic;

namespace Project;

[Tool]
public partial class AnimatedVBoxContainer : Control
{
	[Export]
	public Animation AnimationType = Animation.SlideLeft;
	private float ChildrenCumulativeHeight;
	private readonly Dictionary<Control, ChildState> childState = new();
	public override void _EnterTree()
	{
		if (!Engine.IsEditorHint())
		{
			foreach (var child in GetChildren())
				RemoveChild(child);
		}

		this.ChildEnteredTree += OnChildEnteredTree;
		this.ChildExitingTree += OnChildExitingTree;
	}

	private void OnChildEnteredTree(Node node)
	{
		if (node is not Control control)
			return;

		childState.TryAdd(control, new ChildState() { Node = control, IsDespawning = false });
		var x = -Size.X - 30;
		if (AnimationType == Animation.SlideRight)
			x = Size.X + 30;
		control.Position = new Vector2(x, Size.Y - ChildrenCumulativeHeight - control.Size.Y);
		SetSize(control);
	}

	private void OnChildExitingTree(Node node)
	{
		if (node is not Control canvasItem)
			return;

		childState.Remove(canvasItem);
	}

	public void FreeChildWithAnimation(Node node)
	{
		if (node is not Control control)
			return;

		childState.TryGetValue(control, out var state);
		state.IsDespawning = true;
	}

	public override void _Process(double delta)
	{
		ChildrenCumulativeHeight = 0;
		for (var i = 0; i < GetChildCount(); i++)
		{
			var control = (Control)GetChild(i);
			var result = childState.TryGetValue(control, out var state);
			if (!result)
				continue;

			Vector2 targetPosition;
			switch (AnimationType)
			{
				case Animation.SlideLeft when state.IsDespawning:
					targetPosition = new Vector2(-control.Size.X * 2 - 10, control.Position.Y);
					break;
				case Animation.SlideRight when state.IsDespawning:
					targetPosition = new Vector2(control.Size.X * 2 + 10, control.Position.Y);
					break;
				default:
					targetPosition = new Vector2(0, Size.Y - ChildrenCumulativeHeight - control.Size.Y);
					ChildrenCumulativeHeight += control.Size.Y + 5;
					break;
			}

			control.Position = control.Position.Lerp(targetPosition, Math.Min(1, (float)delta * 5)).Lerp(targetPosition, Math.Min(1, (float)delta * 5));
			if (state.IsDespawning && (control.Position.X <= -control.Size.X * 2 || control.Position.X > control.Size.X * 2))
				control.QueueFree();
		}
	}

	private async void SetSize(Control control)
	{
		await ToSignal(GetTree().CreateTimer(0), "timeout".ToStringName());
		control.Size = new Vector2(Size.X, control.GetMinimumSize().Y);
	}

	private class ChildState
	{
		public CanvasItem Node;
		public bool IsDespawning;
	}

	public enum Animation
	{
		SlideLeft,
		SlideRight,
	}
}
