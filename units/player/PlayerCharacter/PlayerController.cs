using System.Collections.Generic;
using System.Linq;

namespace Project;

public partial class PlayerController : BaseUnit
{
	public PlayerMovement Movement;
	public PlayerTargeting Targeting;
	public PlayerSpellcasting Spellcasting;

	public override void _Ready()
	{
		base._Ready();

		FriendlyName = "The Player";
		Alliance = UnitAlliance.Player;
		Targetable.selectionRadius = 0.25f;

		Health.SetBaseMaxValue(250);
		Health.Regeneration = 1;
		Mana.SetBaseMaxValue(100);
		Mana.SetMinValue(-250);
		Mana.Regeneration = 5;

		Movement = new PlayerMovement(this);
		Targeting = new PlayerTargeting(this);
		Spellcasting = new PlayerSpellcasting(this);

		AddChild(Movement);
		AddChild(Targeting);
		AddChild(Spellcasting);
	}

	protected override void HandleDeath()
	{
		Buffs.Add(new BuffRigorMortis());
	}

	protected override void ProcessGravity(double delta)
	{
		base.ProcessGravity(delta);
		if (Grounded)
			Movement.ResetJumpCount();
	}

	public static List<PlayerController> AllPlayers
	{
		get => AllUnits.Where(unit => unit is PlayerController).Cast<PlayerController>().ToList();
	}
}
