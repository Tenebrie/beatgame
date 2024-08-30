using Godot;

namespace Project;

public partial class BaseCastAutoAttack : Node
{
	BaseCast Parent;

	public bool IsAutoCasting;

	public BaseCastAutoAttack(BaseCast parent)
	{
		Parent = parent;
	}

	public override void _Ready()
	{
		Music.Singleton.BeatTick += OnBeatTick;
		SignalBus.Singleton.CastAssignedAsAuto += OnCastAssignedAsAuto;
	}

	public override void _ExitTree()
	{
		Music.Singleton.BeatTick -= OnBeatTick;
		SignalBus.Singleton.CastAssignedAsAuto -= OnCastAssignedAsAuto;
	}

	void OnBeatTick(BeatTime time)
	{
		if (!IsAutoCasting || time.IsNot(BeatTime.EveryFullBeat))
			return;

		if (Parent.Parent is not PlayerController player)
			return;

		var queuedCast = player.Spellcasting.CastQueue.GetCurrentQueuedCast();
		var isCastingSomething = player.Spellcasting.GetCurrentCastingSpell() != null || (queuedCast != null && queuedCast.Settings.GlobalCooldown != GlobalCooldownMode.Ignore);
		if (isCastingSomething)
			return;

		var targetData = player.Spellcasting.GetTargetData();
		var castQueueMode = Parent.ValidateIfCastIsPossible(targetData, out var errorMessage);
		if (castQueueMode != BaseCast.CastQueueMode.Instant)
			return;

		Parent.CastBegin(targetData, timeGraceGiven: 0);
	}

	void OnCastAssignedAsAuto(BaseCast cast)
	{
		if (cast != Parent)
			IsAutoCasting = false;
	}

	public void Toggle()
	{
		if (IsAutoCasting)
			Unregister();
		else
			Register();
	}

	public void Register()
	{
		IsAutoCasting = true;
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CastAssignedAsAuto, Parent);
	}

	public void Unregister()
	{
		IsAutoCasting = false;
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CastUnassignedAsAuto, Parent);
	}
}