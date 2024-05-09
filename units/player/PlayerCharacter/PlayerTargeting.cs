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
		SignalBus.GetInstance(this).ObjectHovered += OnObjectHovered;
		SignalBus.GetInstance(this).ObjectTargeted += OnObjectTargeted;
		SignalBus.GetInstance(this).UnitDestroyed += OnUnitDestroyed;
	}

	public override void _ExitTree()
	{
		SignalBus.GetInstance(this).ObjectHovered -= OnObjectHovered;
		SignalBus.GetInstance(this).ObjectTargeted -= OnObjectTargeted;
		SignalBus.GetInstance(this).UnitDestroyed -= OnUnitDestroyed;
	}

	private void OnObjectHovered(BaseUnit unit)
	{
		hoveredUnit = unit;
	}

	private void OnObjectTargeted(BaseUnit unit)
	{
		targetedUnit = unit;
	}

	private void OnUnitDestroyed(BaseUnit unit)
	{
		if (unit == targetedUnit)
			targetedUnit = null;
		if (unit == hoveredUnit)
			hoveredUnit = null;
	}

	// TODO: Refactor everything below this line

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ShiftCast1", exactMatch: true))
		{
			var scene = GD.Load<PackedScene>("res://effects/HealImpact/HealImpact.tscn");
			var healImpact = scene.Instantiate() as ProjectileImpact;
			healImpact.AttachForDuration(Parent, .3f, new Vector3(0, 0.25f, 0));

			Parent.Health.Restore(10);
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
		if (@event.IsActionPressed("Cast1", exactMatch: true))
		{
			var scene = GD.Load<PackedScene>("res://effects/FireballProjectile/FireballProjectile.tscn");
			var fireball = scene.Instantiate() as Projectile;
			GetTree().Root.AddChild(fireball);
			fireball.GlobalPosition = GlobalPosition + new Vector3(0, 0.5f, 0);
			fireball.TargetUnit = targetedUnit;
		}
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