using Godot;

namespace Project;

public partial class CheatsController : Node
{
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("GodMode".ToStringName(), exactMatch: true))
		{
			var player = PlayerController.AllPlayers[0];
			if (player.Buffs.Has<BuffDebugInvulnerability>())
			{
				GD.PushWarning("GodMode disabled");
				player.Buffs.RemoveAll<BuffDebugInvulnerability>();
			}
			else
			{
				GD.PushWarning("GodMode enabled");
				player.Buffs.Add(new BuffDebugInvulnerability());
			}
		}
	}
}