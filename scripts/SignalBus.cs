using Godot;

namespace Project;
public partial class SignalBus : Node
{
	[Signal] public delegate void CameraMovingStartedEventHandler();
	[Signal] public delegate void CameraMovingFinishedEventHandler();
	/// <summary>
	/// 	Unit has just entered a scene tree
	/// </summary>
	[Signal] public delegate void UnitCreatedEventHandler(BaseUnit unit);
	/// <summary>
	/// 	Unit's Health has been brought to 0, or an instakill has been called.
	/// 	Most units emit UnitDestroyed the next frame after that, but some (player and bosses) suppress that event until later.
	/// </summary>
	[Signal] public delegate void UnitKilledEventHandler(BaseUnit unit);
	/// <summary>
	/// 	Unit is about to leave the scene tree completely
	/// </summary>
	[Signal] public delegate void UnitDestroyedEventHandler(BaseUnit unit);
	[Signal] public delegate void ObjectHoveredEventHandler(BaseUnit unit);
	[Signal] public delegate void ObjectTargetedEventHandler(BaseUnit unit, TargetedUnitAlliance alliance);
	[Signal] public delegate void ObjectUntargetedEventHandler(TargetedUnitAlliance alliance);
	[Signal] public delegate void DamageTakenEventHandler(BuffIncomingDamageVisitor data);
	[Signal] public delegate void SilentDamageTakenEventHandler(BuffIncomingDamageVisitor data, SilentDamageReason damageType);
	[Signal] public delegate void ResourceChangedEventHandler(BaseUnit unit, ObjectResourceType type, float value);
	[Signal] public delegate void ResourceRegeneratedEventHandler(BaseUnit unit, ObjectResourceType type, float value);
	[Signal] public delegate void MaxResourceChangedEventHandler(BaseUnit unit, ObjectResourceType type, float value);
	[Signal] public delegate void CastStartedEventHandler(BaseCast cast);
	[Signal] public delegate void CastCompletedEventHandler(BaseCast cast);
	[Signal] public delegate void CastInterruptedEventHandler(BaseCast cast);
	[Signal] public delegate void CastFailedEventHandler(BaseCast cast);
	[Signal] public delegate void CastHoveredEventHandler(BaseCast cast);
	[Signal] public delegate void CastUnhoveredEventHandler(BaseCast cast);
	[Signal] public delegate void BuffHoveredEventHandler(BaseBuff buff);
	[Signal] public delegate void BuffUnhoveredEventHandler(BaseBuff buff);
	[Signal] public delegate void SkillHoveredEventHandler(BaseSkill skill);
	[Signal] public delegate void SkillUnhoveredEventHandler(BaseSkill skill);
	[Signal] public delegate void TrackStartedEventHandler(MusicTrack track);
	[Signal] public delegate void SceneTransitionStartedEventHandler(PackedScene scene);
	[Signal] public delegate void SceneTransitionFinishedEventHandler(PackedScene scene);
	[Signal] public delegate void SceneTransitionMusicReadyEventHandler();
	[Signal] public delegate void SceneFadeOutFinishedEventHandler();
	[Signal] public delegate void SceneFadeInFinishedEventHandler();

	/// <summary>
	/// Cast assigned to a slot in PlayerSpellcasting
	/// </summary>
	/// <param name="cast">Reference to the assigned cast</param>
	/// <param name="actionName">Input name for the cast</param>
	[Signal] public delegate void CastAssignedEventHandler(BaseCast cast, string actionName);
	/// <summary>
	/// Cast unassigned from a slot in PlayerSpellcasting
	/// </summary>
	/// <param name="cast">Reference to the unassigned cast</param>
	/// <param name="actionName">Input name for the cast</param>
	[Signal] public delegate void CastUnassignedEventHandler(BaseCast cast, string actionName);

	[Signal]
	public delegate void MessageSentEventHandler(string text, float duration);

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

	public static void SendMessage(string text, float duration = 2)
	{
		instance.EmitSignal(SignalName.MessageSent, text, duration);
	}

	private static SignalBus instance = null;
	public static SignalBus Singleton
	{
		get => instance;
	}
}