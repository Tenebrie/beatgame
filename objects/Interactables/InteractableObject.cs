using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project;

public abstract partial class InteractableObject : Area3D
{
	[Export(PropertyHint.MultilineText)]
	public string labelText;

	bool IsHovered = false;
	bool IsPressed = false;
	float hoverHighlight = 0.0f;

	readonly List<MeshInstance3D> meshes = new();

	public override void _Ready()
	{
		RegisterAllMeshes(this.GetParent());
	}

	void RegisterAllMeshes(Node node)
	{
		if (node is MeshInstance3D mesh)
			meshes.Add(mesh);

		foreach (var child in node.GetChildren().Cast<Node>())
		{
			RegisterAllMeshes(child);
		}
	}

	public override void _EnterTree()
	{
		SignalBus.Singleton.CameraMovingStarted += OnCameraMovingStarted;
	}

	public override void _ExitTree()
	{
		SignalBus.Singleton.CameraMovingStarted -= OnCameraMovingStarted;
	}

	void OnCameraMovingStarted()
	{
		IsPressed = false;
	}

	public override void _MouseEnter()
	{
		IsHovered = true;
		SetHoverHighlight(1.0f);

		if (labelText != null && labelText.Length > 0 && (PlayerController.AllPlayers.Count == 0 || !PlayerController.AllPlayers[0].Movement.IsMovingCamera()))
			InteractableObjectPopup.Singleton.AddObject(this);
	}

	public override void _MouseExit()
	{
		IsHovered = false;
		InteractableObjectPopup.Singleton.RemoveObject(this);
	}

	public override void _Process(double delta)
	{
		if (IsHovered || IsPressed)
			SetHoverHighlight(Math.Min(1.0f, hoverHighlight + 5.00f * (float)delta));
		else
			SetHoverHighlight(Math.Max(0.0f, hoverHighlight - 5.00f * (float)delta));
	}

	void SetHoverHighlight(float value)
	{
		hoverHighlight = value;
		foreach (var mesh in meshes)
			mesh.SetInstanceShaderParameter("hover_highlight", hoverHighlight);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (IsHovered && @event.IsActionPressed("MouseInteractAlt"))
		{
			IsPressed = true;
		}
		if (IsPressed && @event.IsActionReleased("MouseInteractAlt"))
		{
			IsPressed = false;
			OnInteract();
		}
	}

	protected virtual void OnInteract() { }
}
