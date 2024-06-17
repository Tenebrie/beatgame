using Godot;

namespace Project;
public partial class SignalBus : Node
{
	[Signal]
	public delegate void CameraMovingStartedEventHandler();
	[Signal]
	public delegate void CameraMovingFinishedEventHandler();
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
	public delegate void ResourceRegeneratedEventHandler(BaseUnit unit, ObjectResourceType type, float value);
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
	public delegate void SkillHoveredEventHandler(BaseSkill skill);
	[Signal]
	public delegate void SkillUnhoveredEventHandler(BaseSkill skill);
	[Signal]
	public delegate void TrackStartedEventHandler(MusicTrack track);
	[Signal]
	public delegate void SceneTransitionStartedEventHandler(PackedScene scene);
	[Signal]
	public delegate void SceneTransitionFinishedEventHandler(PackedScene scene);
	[Signal]
	public delegate void SceneTransitionMusicReadyEventHandler();
	[Signal]
	public delegate void SceneFadeOutFinishedEventHandler();
	[Signal]
	public delegate void SceneFadeInFinishedEventHandler();

	// Cast assigned to a slot in PlayerSpellcasting
	[Signal]
	public delegate void CastAssignedEventHandler(BaseCast cast, string actionName);
	[Signal]
	public delegate void CastUnassignedEventHandler(BaseCast cast, string actionName);

	[Signal]
	public delegate void MessageSentEventHandler(string text);

	public override void _EnterTree()
	{
		instance = this;
		GD.Randomize();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("MouseInteract") && !Input.IsActionPressed("HardCameraMove"))
		{
			EmitSignal(SignalName.ObjectUntargeted);
		}
	}

	public static void SendMessage(string text)
	{
		instance.EmitSignal(SignalName.MessageSent, text);
	}

	private static SignalBus instance = null;
	public static SignalBus Singleton
	{
		get => instance;
	}
}