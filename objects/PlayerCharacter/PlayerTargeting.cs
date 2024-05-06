using Godot;
using System;
using System.Diagnostics;

namespace Project;

public partial class PlayerTargeting : Node
{
	[Export] public NodePath _mainCameraPath = null;

	Camera3D mainCamera;
	PlayerController parent;

	public BaseUnit hoveredUnit;
	public BaseUnit targetedUnit;

	public override void _Ready()
	{
		parent = GetParent<PlayerController>();
		mainCamera = GetNode<Camera3D>(_mainCameraPath);

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
		if (targetedUnit == null)
		{
			return;
		}
		if (@event.IsActionPressed("Cast1"))
		{
			var scene = GD.Load<PackedScene>("res://objects/FireballProjectile/FireballProjectile.tscn");
			var fireball = scene.Instantiate() as Projectile;
			GetTree().Root.AddChild(fireball);
			fireball.GlobalPosition = GetParent<Node3D>().GlobalPosition + new Vector3(0, 0.5f, 0);
			fireball.targetUnit = targetedUnit;
		}
	}
}