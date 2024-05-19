using System.Diagnostics;
using Godot;

namespace Project;

public partial class PlayerTargeting : ComposableScript
{
	new readonly PlayerController Parent;

	public BaseUnit hoveredUnit;
	public BaseUnit targetedUnit;
	Timer repeatCastTimer;

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

		// TODO: Refactor everything below this line
		if (@event.IsActionPressed("ShiftCast1", exactMatch: true))
		{

		}

		if (@event.IsActionPressed("Cast2", exactMatch: true))
		{
			if (repeatCastTimer != null)
			{
				return;
			}
			repeatCastTimer = new Timer();
			Parent.AddChild(repeatCastTimer);
			repeatCastTimer.Timeout += OnRepeatCastTimer;
			repeatCastTimer.WaitTime = 0.02;
			repeatCastTimer.Start();
		}
		if (@event.IsActionReleased("Cast2", exactMatch: true))
		{
			if (repeatCastTimer == null)
			{
				return;
			}
			repeatCastTimer.Stop();
			repeatCastTimer.QueueFree();
			repeatCastTimer = null;
		}

		if (targetedUnit == null)
		{
			return;
		}

		// TODO: Better casting please
		// if (@event.IsActionPressed("Cast1", exactMatch: true))
		// {
		// 	var scene = GD.Load<PackedScene>("res://effects/FireballProjectile/FireballProjectile.tscn");
		// 	var fireball = scene.Instantiate() as Projectile;
		// 	GetTree().Root.AddChild(fireball);
		// 	fireball.GlobalPosition = GlobalPosition + new Vector3(0, 0.5f, 0);
		// 	fireball.TargetUnit = targetedUnit;
		// }
	}

	private void OnRepeatCastTimer()
	{
		var scene = GD.Load<PackedScene>("res://effects/Flamethrower/FlamethrowerProjectile.tscn");
		var fireball = scene.Instantiate() as TrueProjectile;
		GetTree().Root.AddChild(fireball);
		fireball.GlobalPosition = GlobalPosition + new Vector3(0, 0.5f, 0);
		fireball.GlobalTransform = GlobalTransform;
		fireball.Position = new Vector3(Position.X, Position.Y + 0.5f, Position.Z);
		fireball.Alliance = UnitAlliance.Player;
	}

	// (this curly brace is fine)
	/*{*/
}