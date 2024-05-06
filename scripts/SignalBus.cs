using Godot;

namespace Project;
public partial class SignalBus : Node
{
	[Signal]
	public delegate void ObjectHoveredEventHandler(BaseUnit unit);
	[Signal]
	public delegate void ObjectTargetedEventHandler(BaseUnit unit);

	public static SignalBus GetInstance(Node node)
	{
		return node.GetNode<SignalBus>("/root/SignalBus");
	}
}