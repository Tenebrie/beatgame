using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace Project;

public partial class PlayerTargeting : ComposableScript
{
	const float MaxRaycastDist = 25;

	new readonly PlayerController Parent;

	Timer updateHoverTimer;
	Camera3D camera;
	public BaseUnit hoveredUnit;
	public BaseUnit targetedAlliedUnit;
	public BaseUnit targetedHostileUnit;

	public PlayerTargeting(BaseUnit parent) : base(parent)
	{
		Parent = parent as PlayerController;
	}

	public override void _Ready()
	{
		camera = Parent.GetComponent<Camera3D>();
		// SignalBus.Singleton.ObjectHovered += OnObjectHovered;
		SignalBus.Singleton.ObjectTargeted += OnObjectTargeted;
		SignalBus.Singleton.ObjectUntargeted += OnObjectUntargeted;
		SignalBus.Singleton.UnitDestroyed += OnUnitDestroyed;

		updateHoverTimer = new()
		{
			Autostart = true,
			WaitTime = 0.03f
		};
		updateHoverTimer.Timeout += OnUpdateHover;
		AddChild(updateHoverTimer);
	}

	public override void _ExitTree()
	{
		// SignalBus.Singleton.ObjectHovered -= OnObjectHovered;
		SignalBus.Singleton.ObjectTargeted -= OnObjectTargeted;
		SignalBus.Singleton.ObjectUntargeted -= OnObjectUntargeted;
		SignalBus.Singleton.UnitDestroyed -= OnUnitDestroyed;
	}

	List<Vector3> GenerateMouseCastPositions(Vector2 mousePos, int offset)
	{
		var diagonalOffset = offset * 1.42f;
		return new()
		{
			camera.ProjectRayNormal(mousePos + new Vector2(+offset, 0)),
			camera.ProjectRayNormal(mousePos + new Vector2(-offset, 0)),
			camera.ProjectRayNormal(mousePos + new Vector2(0, +offset)),
			camera.ProjectRayNormal(mousePos + new Vector2(0, -offset)),
			camera.ProjectRayNormal(mousePos + new Vector2(+diagonalOffset, +diagonalOffset)),
			camera.ProjectRayNormal(mousePos + new Vector2(-diagonalOffset, +diagonalOffset)),
			camera.ProjectRayNormal(mousePos + new Vector2(-diagonalOffset, -diagonalOffset)),
			camera.ProjectRayNormal(mousePos + new Vector2(+diagonalOffset, -diagonalOffset)),
		};
	}

	bool ProcessRaycastResult(Vector3 cameraOrigin, Vector3 cameraNormal)
	{
		var targetPosition = cameraOrigin + cameraNormal * MaxRaycastDist;
		var hitUnit = Raycast.GetFirstHitUnitGlobal(Parent, cameraOrigin, targetPosition, Raycast.Layer.Hoverable);
		if (hitUnit == null)
			return false;

		HoverUnit(hitUnit);
		return true;
	}

	void OnUpdateHover()
	{
		var mousePos = GetViewport().GetMousePosition();
		var cameraOrigin = camera.ProjectRayOrigin(mousePos);
		var cameraNormal = camera.ProjectRayNormal(mousePos);

		if (ProcessRaycastResult(cameraOrigin, cameraNormal))
			return;

		var firstRoundPositions = GenerateMouseCastPositions(mousePos, 5);
		foreach (var basePos in firstRoundPositions)
			if (ProcessRaycastResult(cameraOrigin, basePos))
				return;

		var secondRoundPositions = GenerateMouseCastPositions(mousePos, 10);
		foreach (var basePos in secondRoundPositions)
			if (ProcessRaycastResult(cameraOrigin, basePos))
				return;

		var thirdRoundPositions = GenerateMouseCastPositions(mousePos, 20);
		foreach (var basePos in thirdRoundPositions)
			if (ProcessRaycastResult(cameraOrigin, basePos))
				return;

		UnhoverUnit(hoveredUnit);
	}

	private void HoverUnit(BaseUnit unit)
	{
		if (hoveredUnit == unit)
			return;

		this.Log("Hover!");
		hoveredUnit = unit;
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.ObjectHovered, hoveredUnit);
	}

	private void UnhoverUnit(BaseUnit unit)
	{
		if (hoveredUnit == null)
			return;

		this.Log("Unhover!");
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.ObjectUnhovered, hoveredUnit);
		hoveredUnit = null;
	}

	private void OnObjectTargeted(BaseUnit unit, TargetedUnitAlliance alliance)
	{
		if (alliance == TargetedUnitAlliance.Player)
			targetedAlliedUnit = unit;
		else
			targetedHostileUnit = unit;
	}

	private void OnObjectUntargeted(TargetedUnitAlliance alliance)
	{
		if (alliance == TargetedUnitAlliance.Player)
			targetedAlliedUnit = null;
		else
			targetedHostileUnit = null;
	}

	private void OnUnitDestroyed(BaseUnit unit)
	{
		if (unit == targetedAlliedUnit)
			targetedAlliedUnit = null;
		if (unit == targetedHostileUnit)
			targetedHostileUnit = null;
		if (unit == hoveredUnit)
			hoveredUnit = null;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (hoveredUnit == null)
			return;

		if (@event.IsActionPressed("MouseInteract".ToStringName()) && !Input.IsActionPressed("HardCameraMove".ToStringName()))
		{
			hoveredUnit.GetComponent<ObjectTargetable>().MakeTargeted();
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("TabTargetPrevious".ToStringName(), exactMatch: true))
		{
			var enemyUnits = BaseUnit.AllUnits.FindAll(unit => unit.Alliance.HostileTo(Parent.Alliance));
			if (enemyUnits.Count == 0)
				return;

			var indexOfCurrentTarget = enemyUnits.IndexOf(targetedHostileUnit);
			if (indexOfCurrentTarget < enemyUnits.Count - 1)
			{
				var nextUnit = enemyUnits[indexOfCurrentTarget + 1];
				nextUnit.Targetable.MakeTargeted();
			}
			else
			{
				enemyUnits[0].Targetable.MakeTargeted();
			}
		}
		if (@event.IsActionPressed("TabTargetNext".ToStringName(), exactMatch: true))
		{
			var enemyUnits = BaseUnit.AllUnits.FindAll(unit => unit.Alliance.HostileTo(Parent.Alliance));
			if (enemyUnits.Count == 0)
				return;

			var indexOfCurrentTarget = enemyUnits.IndexOf(targetedHostileUnit);
			if (indexOfCurrentTarget > 0)
			{
				var nextUnit = enemyUnits[indexOfCurrentTarget - 1];
				nextUnit.Targetable.MakeTargeted();
			}
			else
			{
				enemyUnits[^1].Targetable.MakeTargeted();
			}
		}
		if (@event.IsActionPressed("TabTargetAllyNext".ToStringName(), exactMatch: true))
		{
			var alliedUnits = BaseUnit.AllUnits.FindAll(unit => unit.Alliance == Parent.Alliance);
			if (alliedUnits.Count == 0)
				return;

			var indexOfCurrentTarget = alliedUnits.IndexOf(targetedAlliedUnit);
			if (indexOfCurrentTarget < alliedUnits.Count - 1)
			{
				var nextUnit = alliedUnits[indexOfCurrentTarget + 1];
				nextUnit.Targetable.MakeTargeted();
			}
			else
			{
				// SignalBus.Singleton.EmitSignal(SignalBus.SignalName.ObjectUntargeted, TargetedUnitAlliance.Player.ToVariant());
				alliedUnits[0].Targetable.MakeTargeted();
			}
		}
	}
}