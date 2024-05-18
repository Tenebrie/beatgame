using Godot;

namespace Project;
public partial class SignalBus : Node
{
	[Signal]
	public delegate void UnitCreatedEventHandler(BaseUnit unit);
	[Signal]
	public delegate void UnitDestroyedEventHandler(BaseUnit unit);
	[Signal]
	public delegate void ObjectHoveredEventHandler(BaseUnit unit);
	[Signal]
	public delegate void ObjectTargetedEventHandler(BaseUnit unit);
	[Signal]
	public delegate void ObjectUntargetedEventHandler();
	[Signal]
	public delegate void ResourceChangedEventHandler(BaseUnit unit, ObjectResourceType type, float value);
	[Signal]
	public delegate void MaxResourceChangedEventHandler(BaseUnit unit, ObjectResourceType type, float value);
	[Signal]
	public delegate void CastStartedEventHandler(BaseCast cast);
	[Signal]
	public delegate void CastCancelledEventHandler(BaseCast cast);
	[Signal]
	public delegate void CastPerformedEventHandler(BaseCast cast);
	[Signal]
	public delegate void CastFailedEventHandler(BaseCast cast);
	[Signal]
	public delegate void TrackStartedEventHandler(MusicTrack track);

	public static SignalBus GetInstance(ComposableScript node)
	{
		return node.Parent.GetNode<SignalBus>("/root/SignalBus");
	}

	public static SignalBus GetInstance(Node node)
	{
		return node.GetNode<SignalBus>("/root/SignalBus");
	}

    public override void _EnterTree()
    {
        instance = this;
    }

    public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("MouseInteract") && !Input.IsActionPressed("HardCameraMove"))
		{
			EmitSignal(SignalName.ObjectUntargeted);
		}
	}

	private static SignalBus instance = null;
	public static SignalBus Singleton
	{
		get => instance;
	}
}