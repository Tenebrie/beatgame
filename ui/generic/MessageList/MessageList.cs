using System.Diagnostics;
using Godot;

namespace Project;

public partial class MessageList : VBoxContainer
{
	public override void _EnterTree()
	{
		foreach (var child in GetChildren())
			RemoveChild(child);

		SignalBus.Singleton.MessageSent += AddMessage;
		AddMessage("Controls: \n- WASD to move\n- Mouse to look around\n- Tab to cycle enemies\n- Mouse 4 to cycle allies\n- K to open the skill tree\n- M to open the DPS meter", 20);
	}

	public void AddMessage(string text, float duration)
	{
		var message = Lib.LoadScene(Lib.UI.MessageListMessage).Instantiate<Message>();
		AddChild(message);
		MoveChild(message, 0);
		message.Body = text;
		message.Duration = duration;
	}
}
