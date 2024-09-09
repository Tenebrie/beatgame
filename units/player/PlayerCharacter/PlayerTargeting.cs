using System.Diagnostics;
using Godot;

namespace Project;

public partial class PlayerTargeting : ComposableScript
{
	new readonly PlayerController Parent;

	Camera3D camera;
	ShapeCast3D raycast;
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
		raycast = new ShapeCast3D();
		AddChild(raycast);
		this.Log(camera);
		// SignalBus.Singleton.ObjectHovered += OnObjectHovered;
		SignalBus.Singleton.ObjectTargeted += OnObjectTargeted;
		SignalBus.Singleton.ObjectUntargeted += OnObjectUntargeted;
		SignalBus.Singleton.UnitDestroyed += OnUnitDestroyed;
	}

	public override void _ExitTree()
	{
		// SignalBus.Singleton.ObjectHovered -= OnObjectHovered;
		SignalBus.Singleton.ObjectTargeted -= OnObjectTargeted;
		SignalBus.Singleton.ObjectUntargeted -= OnObjectUntargeted;
		SignalBus.Singleton.UnitDestroyed -= OnUnitDestroyed;
	}

	public override void _PhysicsProcess(double delta)
	{
		var mousePos = GetViewport().GetMousePosition();
		var from = camera.ProjectRayOrigin(mousePos);
		var to = from + camera.ProjectRayNormal(mousePos) * 30;

		raycast.GlobalPosition = from;
		raycast.TargetPosition = raycast.ToLocal(to);
		raycast.CollisionMask = Raycast.Layer.Hoverable.AsUnsignedInt();
		var shape = new SphereShape3D
		{
			Radius = 0.25f
		};
		raycast.Shape = shape;
		raycast.ForceShapecastUpdate();
		for (var i = 0; i < raycast.GetCollisionCount(); i++)
		{
			var collider = raycast.GetCollider(i);
			if (collider is BaseUnit unit)
			{
				HoverUnit(unit);
				return;
			}
		}

		if (raycast.GetCollisionCount() == 0)
		{
			UnhoverUnit(hoveredUnit);
		}
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