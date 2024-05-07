using Godot;

namespace Project;
public partial class SignalBus : Node
{
	[Signal]
	public delegate void ObjectHoveredEventHandler(BaseUnit unit);
	[Signal]
	public delegate void ObjectTargetedEventHandler(BaseUnit unit);
	[Signal]
	public delegate void ResourceChangedEventHandler(BaseUnit unit, ObjectResourceType type, float value);
	[Signal]
	public delegate void MaxResourceChangedEventHandler(BaseUnit unit, ObjectResourceType type, float value);

	public static SignalBus GetInstance(Node node)
	{
		return node.GetNode<SignalBus>("/root/SignalBus");
	}
}