using System.Diagnostics;
using Godot;

namespace Project;

public partial class PlayerTargeting : ComposableScript
{
	new readonly PlayerController Parent;

	public BaseUnit hoveredUnit;
	public BaseUnit targetedUnit;

	public PlayerTargeting(BaseUnit parent) : base(parent)
	{
		Parent = parent as PlayerController;
	}

	public override void _Ready()
	{
		SignalBus.Singleton.ObjectHovered += OnObjectHovered;
		SignalBus.Singleton.ObjectTargeted += OnObjectTargeted;
		SignalBus.Singleton.ObjectUntargeted += OnObjectUntargeted;
		SignalBus.Singleton.UnitDestroyed += OnUnitDestroyed;
	}

	public override void _ExitTree()
	{
		SignalBus.Singleton.ObjectHovered -= OnObjectHovered;
		SignalBus.Singleton.ObjectTargeted -= OnObjectTargeted;
		SignalBus.Singleton.ObjectUntargeted -= OnObjectUntargeted;
		SignalBus.Singleton.UnitDestroyed -= OnUnitDestroyed;
	}

	private void OnObjectHovered(BaseUnit unit)
	{
		hoveredUnit = unit;
	}

	private void OnObjectTargeted(BaseUnit unit)
	{
		targetedUnit = unit;
	}

	private void OnObjectUntargeted()
	{
		targetedUnit = null;
	}

	private void OnUnitDestroyed(BaseUnit unit)
	{
		if (unit == targetedUnit)
			targetedUnit = null;
		if (unit == hoveredUnit)
			hoveredUnit = null;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("TabTargetNext", exactMatch: true))
		{
			var enemyUnits = BaseUnit.AllUnits.FindAll(unit => unit.Alliance.HostileTo(Parent.Alliance));
			if (enemyUnits.Count == 0)
				return;

			var indexOfCurrentTarget = enemyUnits.IndexOf(targetedUnit);
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
		if (@event.IsActionPressed("TabTargetPrevious", exactMatch: true))
		{
			var enemyUnits = BaseUnit.AllUnits.FindAll(unit => unit.Alliance.HostileTo(Parent.Alliance));
			if (enemyUnits.Count == 0)
				return;

			var indexOfCurrentTarget = enemyUnits.IndexOf(targetedUnit);
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
	}
}