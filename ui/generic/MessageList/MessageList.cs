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
	}

	public void AddMessage(string text)
	{
		var message = Lib.LoadScene(Lib.UI.MessageListMessage).Instantiate<Message>();
		AddChild(message);
		MoveChild(message, 0);
		message.Body = text;
	}
}
