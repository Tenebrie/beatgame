using System;
using System.Diagnostics;
using Godot;
namespace Project;

public partial class ObjectTargetable : ComposableScript
{
	public bool Untargetable = false;

	public bool isHovered;
	public bool isTargeted;
	public TargetedUnitAlliance? targetedAs;
	public float hoverHighlight;

	public TargetingCircle selectionModel = null;

	public float SelectionRadius = 1f;

	public ObjectTargetable(BaseUnit parent) : base(parent) { }

	public override void _Ready()
	{
		SignalBus.Singleton.ObjectHovered += OnObjectHovered;
		SignalBus.Singleton.ObjectUnhovered += OnObjectUnhovered;
		SignalBus.Singleton.ObjectTargeted += OnObjectTargeted;
	}

	public override void _ExitTree()
	{
		SignalBus.Singleton.ObjectHovered -= OnObjectHovered;
		SignalBus.Singleton.ObjectUnhovered -= OnObjectUnhovered;
		SignalBus.Singleton.ObjectTargeted -= OnObjectTargeted;
	}

	public override void _Process(double delta)
	{
		if (isHovered)
		{
			hoverHighlight = Math.Min(1.0f, hoverHighlight + 5.00f * (float)delta);
			UpdateHoverHighlight();
		}
		else
		{
			hoverHighlight = Math.Max(0.0f, hoverHighlight - 5.00f * (float)delta);
			UpdateHoverHighlight();
		}
	}

	// public override void _UnhandledInput(InputEvent @event)
	// {
	// 	if (@event.IsActionPressed("MouseInteract".ToStringName()) && !Input.IsActionPressed("HardCameraMove".ToStringName()))
	// 	{
	// 		SetTargeted(false, targetedAs);
	// 	}
	// }

	private void UpdateHoverHighlight()
	{
		for (var i = 0; i < Parent.GetChildCount(); i++)
		{
			var child = Parent.GetChild(i);
			if (child is MeshInstance3D meshChild)
			{
				meshChild.SetInstanceShaderParameter("hover_highlight".ToStringName(), hoverHighlight);
			}
		}
		// TODO: Performance fix pls
		Parent.GetComponentsUncached<MeshInstance3D>().ForEach(comp => comp.SetInstanceShaderParameter("hover_highlight".ToStringName(), hoverHighlight));
	}

	private void OnObjectTargeted(BaseUnit unit, TargetedUnitAlliance alliance)
	{
		if (isTargeted && alliance != targetedAs)
			return;

		if (Parent is not PlayerController)
			SetTargeted(unit == Parent, alliance);
	}

	private void OnObjectHovered(BaseUnit unit)
	{
		if (isHovered && unit != Parent)
			isHovered = false;

		if (unit != Parent)
			return;

		isHovered = true;
		hoverHighlight = 1.0f;
	}

	private void OnObjectUnhovered(BaseUnit _)
	{
		isHovered = false;
	}

	private void SetTargeted(bool targeted, TargetedUnitAlliance? alliance)
	{
		if (isTargeted == targeted)
			return;

		isTargeted = targeted;
		targetedAs = alliance;

		if (targeted)
		{
			selectionModel = Lib.LoadScene(Lib.Effect.TargetingCircle).Instantiate() as TargetingCircle;
			Parent.AddChild(selectionModel);
			selectionModel.SetAlliance(Parent.Alliance);
			selectionModel.SetRadius(SelectionRadius);
		}
		else if (selectionModel != null && IsInstanceValid(selectionModel))
		{
			targetedAs = null;
			Parent.RemoveChild(selectionModel);
			selectionModel.QueueFree();
			selectionModel = null;
		}
	}

	public void MakeTargeted()
	{
		// Treat neutral targets as hostile
		var alliance = Parent.Alliance switch
		{
			UnitAlliance.Player => TargetedUnitAlliance.Player,
			UnitAlliance.Neutral => TargetedUnitAlliance.Hostile,
			UnitAlliance.Hostile => TargetedUnitAlliance.Hostile,
			_ => throw new NotImplementedException(),
		};
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.ObjectTargeted, Parent, alliance.ToVariant());
	}
}
