using System;
using System.Diagnostics;
using Godot;
namespace Project;

public partial class ObjectTargetable : ComposableScript
{
	public bool isHovered = false;
	public bool isTargeted = false;
	public float hoverHighlight = 0.00f;

	public TargetingCircle selectionModel = null;
	public float selectionRadius = 1f;

	public ObjectTargetable(BaseUnit parent) : base(parent) { }

	public override void _Ready()
	{
		Parent.InputEvent += OnInputEvent;
		Parent.MouseEntered += OnMouseEntered;
		Parent.MouseExited += OnMouseExited;
		SignalBus.GetInstance(Parent).ObjectTargeted += OnObjectTargeted;
	}

	public override void _ExitTree()
	{
		SignalBus.GetInstance(Parent).ObjectTargeted -= OnObjectTargeted;
	}

	public override void _Process(double delta)
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

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("MouseInteract") && !Input.IsActionPressed("HardCameraMove"))
		{
			SetTargeted(false);
		}
	}

	private void UpdateHoverHighlight()
	{
		var children = Parent.GetChildren();
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
		SetTargeted(unit == Parent);
	}

	private void OnMouseEntered()
	{
		SignalBus.GetInstance(Parent).EmitSignal(SignalBus.SignalName.ObjectHovered, Parent);
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
			var scene = GD.Load<PackedScene>("res://effects/TargetingCircle/TargetingCircle.tscn");
			selectionModel = scene.Instantiate() as TargetingCircle;
			selectionModel.SetAlliance(Parent.Alliance);
			selectionModel.SetRadius(selectionRadius);
			Parent.AddChild(selectionModel);
		}
		else if (selectionModel != null && GodotObject.IsInstanceValid(selectionModel))
		{
			Parent.RemoveChild(selectionModel);
			selectionModel.QueueFree();
			selectionModel = null;
		}
	}

	private void OnInputEvent(Node camera, InputEvent @event, Vector3 position, Vector3 normal, long shapeIdx)
	{
		if (@event.IsActionPressed("MouseInteract") && !Input.IsActionPressed("HardCameraMove"))
		{
			SignalBus.GetInstance(Parent).EmitSignal(SignalBus.SignalName.ObjectTargeted, Parent);
		}
	}
}
