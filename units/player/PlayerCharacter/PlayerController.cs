using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace Project;

public partial class PlayerController : BaseUnit
{
	public PlayerMovement Movement;
	public PlayerTargeting Targeting;
	public PlayerSpellcasting Spellcasting;
	public PlayerController()
	{
		FriendlyName = "The Player";
		Alliance = UnitAlliance.Player;
		Targetable.selectionRadius = 0.25f;

		Health.SetBaseMaxValue(250);
		Health.Regeneration = 1;
		Mana.SetBaseMaxValue(100);
		Mana.SetMinValue(-250);
		Mana.Regeneration = 5;
	}

	public override void _Ready()
	{
		Movement = new PlayerMovement(this);
		Targeting = new PlayerTargeting(this);
		Spellcasting = new PlayerSpellcasting(this);

		Composables.Add(Movement);
		Composables.Add(Targeting);
		Composables.Add(Spellcasting);

		AllPlayers.Add(this);
		base._Ready();
	}

	public override void _ExitTree()
	{
		AllPlayers.Remove(this);
		base._ExitTree();
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
