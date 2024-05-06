using Godot;

namespace Project;

public partial class PlayerController : BaseUnit
{
	public PlayerController()
	{
		alliance = Alliance.Player;
		targeting.selectionRadius = 0.5f;
	}
}
