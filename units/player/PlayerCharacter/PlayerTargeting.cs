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
		SignalBus.GetInstance(this).ObjectHovered += OnObjectHovered;
		SignalBus.GetInstance(this).ObjectTargeted += OnObjectTargeted;
	}

	public override void _ExitTree()
	{
		SignalBus.GetInstance(this).ObjectHovered -= OnObjectHovered;
		SignalBus.GetInstance(this).ObjectTargeted -= OnObjectTargeted;
	}

	private void OnObjectHovered(BaseUnit unit)
	{
		hoveredUnit = unit;
	}

	private void OnObjectTargeted(BaseUnit unit)
	{
		targetedUnit = unit;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ShiftCast1", exactMatch: true))
		{
			var scene = GD.Load<PackedScene>("res://effects/HealImpact/HealImpact.tscn");
			var healImpact = scene.Instantiate() as ProjectileImpact;
			GetTree().Root.AddChild(healImpact);
			healImpact.GlobalPosition = GlobalPosition + new Vector3(0, 0.5f, 0);

			Parent.Health.Restore(10);
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
			fireball.targetUnit = targetedUnit;
		}
	}
}