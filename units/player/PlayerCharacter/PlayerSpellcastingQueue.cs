using Godot;

namespace Project;

public partial class PlayerSpellcastingQueue : ComposableScript
{
	new readonly PlayerController Parent;

	public BaseCast CurrentCast;
	Timer CastTimer;

	public PlayerSpellcastingQueue(BaseUnit parent) : base(parent)
	{
		Parent = parent as PlayerController;
	}

	public override void _Ready()
	{
		CastTimer = new Timer { WaitTime = 0.05f };
		CastTimer.Timeout += OnTimeout;
		AddChild(CastTimer);
	}

	public BaseCast GetCurrentQueuedCast()
	{
		return IsInstanceValid(CurrentCast) ? CurrentCast : null;
	}

	void OnTimeout()
	{
		if (CurrentCast == null)
			return;

		if (!IsInstanceValid(CurrentCast))
		{
			CurrentCast = null;
			return;
		}

		var currentCastingSpell = Parent.Spellcasting.GetCurrentCastingSpell();
		if (currentCastingSpell != null)
			return;

		var targetData = Parent.Spellcasting.GetTargetData();
		var castQueueMode = CurrentCast.ValidateIfCastIsPossible(targetData, out _);
		if (castQueueMode == BaseCast.CastQueueMode.None)
		{
			Unqueue();
			return;
		}
		else if (castQueueMode == BaseCast.CastQueueMode.Instant)
		{
			Parent.Spellcasting.CastInputPressed(CurrentCast);
		}
	}

	public void Enqueue(BaseCast cast)
	{
		CastTimer.Start();
		CurrentCast = cast;
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CastQueued, cast);
	}

	public void Unqueue()
	{
		CastTimer.Stop();
		if (CurrentCast == null || !IsInstanceValid(CurrentCast))
			return;

		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CastUnqueued, CurrentCast);
		CurrentCast = null;
	}
}