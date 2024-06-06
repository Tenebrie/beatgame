using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace Project;

public partial class PlayerController : BaseUnit
{
	[Export] public NodePath _mainCameraPath = null;
	[Export] public NodePath _baseCameraPath = null;

	public PlayerMovement Movement;
	public PlayerTargeting Targeting;
	public PlayerSpellcasting Spellcasting;
	public PlayerController()
	{
		FriendlyName = "The Player";
		Alliance = UnitAlliance.Player;
		Targetable.selectionRadius = 0.5f;

		Health.SetMax(250);
		Mana.SetMax(100);
	}

	public override void _Ready()
	{
		Movement = new PlayerMovement(this, _mainCameraPath, _baseCameraPath);
		Targeting = new PlayerTargeting(this);
		Spellcasting = new PlayerSpellcasting(this);

		Composables.Add(Movement);
		Composables.Add(Targeting);
		Composables.Add(Spellcasting);

		Spellcasting.Bind("Cast1", new Fireball(this));
		Spellcasting.Bind("Cast2", new Fireblast(this));
		Spellcasting.Bind("Cast3", new CastZap(this));
		Spellcasting.Bind("ShiftCast1", new SelfHeal(this));
		Spellcasting.Bind("ShiftCast2", new CastRescue(this));

		AllPlayers.Add(this);
		base._Ready();
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		AllPlayers.Remove(this);
	}

	protected override void ProcessGravity(double delta)
	{
		base.ProcessGravity(delta);
		if (Grounded)
		{
			Movement.ResetJumpCount();
		}
	}

	public static readonly List<PlayerController> AllPlayers = new();
}
