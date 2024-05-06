using System;
using System.Diagnostics;
using Godot;
namespace Project;

public partial class ObjectTargeting
{
	public bool isHovered = false;
	public bool isTargeted = false;
	public float hoverHighlight = 0.00f;

	public TargetingCircle selectionModel = null;
	public float selectionRadius = 1f;

	private BaseUnit parent;
	public void Ready(BaseUnit node)
	{
		parent = node;
		node.InputEvent += OnInputEvent;
		node.MouseEntered += OnMouseEntered;
		node.MouseExited += OnMouseExited;
		SignalBus.GetInstance(parent).ObjectTargeted += OnObjectTargeted;
	}

	public void ExitTree()
	{
		SignalBus.GetInstance(parent).ObjectTargeted -= OnObjectTargeted;
	}

	public void Process(double delta)
	{
		if (isHovered)
		{
			hoverHighlight = Math.Min(1.0f, hoverHighlight + 5.00f * (float)delta);
			UpdateHoverHighlight();
		}
		else if (!isTargeted)
		{
			hoverHighlight = Math.Max(0.0f, hoverHighlight - 5.00f * (float)delta);
			UpdateHoverHighlight();
		}
	}

	public void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("MouseInteract") && !Input.IsActionPressed("HardCameraMove"))
		{
			SetTargeted(false);
		}
	}

	private void UpdateHoverHighlight()
	{
		var children = parent.GetChildren();
		foreach (var child in children)
		{
			if (child is MeshInstance3D meshChild)
			{
				meshChild.SetInstanceShaderParameter("hover_highlight", hoverHighlight);
			}
		}
	}

	private void OnObjectTargeted(BaseUnit unit)
	{
		SetTargeted(unit == parent);
	}

	private void OnMouseEntered()
	{
		SignalBus.GetInstance(parent).EmitSignal(SignalBus.SignalName.ObjectHovered, parent);
		isHovered = true;
		hoverHighlight = 1.0f;
	}

	private void OnMouseExited()
	{
		isHovered = false;
	}

	private void SetTargeted(bool targeted)
	{
		if (isTargeted == targeted)
		{
			return;
		}

		isTargeted = targeted;

		if (targeted)
		{
			var scene = GD.Load<PackedScene>("res://objects/utils/TargetingCircle.tscn");
			selectionModel = scene.Instantiate() as TargetingCircle;
			selectionModel.SetAlliance(parent.alliance);
			selectionModel.SetRadius(selectionRadius);
			parent.AddChild(selectionModel);
		}
		else if (selectionModel != null)
		{
			parent.RemoveChild(selectionModel);
			selectionModel.QueueFree();
			selectionModel = null;
		}
	}

	private void OnInputEvent(Node camera, InputEvent @event, Vector3 position, Vector3 normal, long shapeIdx)
	{
		if (@event.IsActionPressed("MouseInteract") && !Input.IsActionPressed("HardCameraMove"))
		{
			SignalBus.GetInstance(parent).EmitSignal(SignalBus.SignalName.ObjectTargeted, parent);
		}
	}
}
