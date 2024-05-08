using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace Project;

public partial class PlayerController : BaseUnit
{
	[Export] public NodePath _mainCameraPath = null;
	[Export] public NodePath _baseCameraPath = null;

	public PlayerMovement Movement;
	public PlayerTargeting PlayerTargeting;
	public PlayerController()
	{
		Alliance = UnitAlliance.Player;
		Targetable.selectionRadius = 0.5f;
	}

	public override void _Ready()
	{
		Movement = new PlayerMovement(this, _mainCameraPath, _baseCameraPath);
		PlayerTargeting = new PlayerTargeting(this);

		Composables.Add(Movement);
		Composables.Add(PlayerTargeting);

		base._Ready();
	}

	protected override void ProcessGravity(double delta)
	{
		base.ProcessGravity(delta);
		if (Grounded)
		{

			Movement.ResetJumpCount();
		}
	}

	public static readonly List<PlayerController> All = new();
}
