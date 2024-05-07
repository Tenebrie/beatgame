using System.Collections.Generic;
using Godot;

namespace Project;

public partial class PlayerController : BaseUnit
{
	public PlayerController()
	{
		alliance = Alliance.Player;
		targeting.selectionRadius = 0.5f;
	}

	public override void _Ready()
	{
		All.Add(this);
	}

	public static readonly List<PlayerController> All = new();
}
