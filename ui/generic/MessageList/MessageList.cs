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
		var message = Lib.Scene(Lib.UI.MessageListMessage).Instantiate<Message>();
		AddChild(message);
		message.Body = text;
	}
}
