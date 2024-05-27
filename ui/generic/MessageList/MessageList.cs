using System.Diagnostics;
using Godot;

namespace Project;

public partial class MessageList : VBoxContainer
{
	private readonly static PackedScene MessageResource = GD.Load<PackedScene>("res://ui/generic/MessageList/Message.tscn");

	// public List<Message> Messages = new();

	public override void _EnterTree()
	{
		foreach (var child in GetChildren())
			RemoveChild(child);

		SignalBus.Singleton.MessageSent += AddMessage;
	}

	public void AddMessage(string text)
	{
		var message = MessageResource.Instantiate<Message>();
		AddChild(message);
		message.Body = text;
	}
}
