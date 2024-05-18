using System.Collections.Generic;
using System.Diagnostics;
using Godot;
namespace Project;

public partial class BaseCast : Node
{
	public Timer RecastTimerHandle;
	public bool IsCasting;
	public long CastStartedAt; // beat index

	public CastInputType InputType = CastInputType.Instant;
	public float HoldTime = 1; // beat
	public CastTargetType TargetType = CastTargetType.None;
	public List<UnitAlliance> TargetAlliances = new() { UnitAlliance.Player, UnitAlliance.Neutral, UnitAlliance.Hostile };
	public BeatTime CastTimings = BeatTime.One;
	public float RecastTime = .1f;
	public bool CastOnFail = true;

	public readonly BaseUnit Parent;

	public BaseCast(BaseUnit parent)
	{
		Parent = parent;
	}

	public override void _ExitTree()
	{
		RemoveChild(RecastTimerHandle);
	}

	private void EnsureTimerExists()
	{
		if (RecastTimerHandle != null)
			return;

		RecastTimerHandle = new Timer
		{
			OneShot = true
		};
		AddChild(RecastTimerHandle);
	}

	public bool ValidateTarget(BaseUnit target, out string errorMessage)
	{
		errorMessage = null;

		if (TargetType == CastTargetType.Unit && target == null)
		{
			errorMessage = "Needs a target";
			return false;
		}
		else if (TargetType == CastTargetType.Unit && !TargetAlliances.Contains(target.Alliance))
		{
			errorMessage = "Can't target this unit";
			return false;
		}

		return true;
	}

	public bool ValidateTiming(out string errorMessage)
	{
		errorMessage = null;

		EnsureTimerExists();
		if (!RecastTimerHandle.IsStopped())
		{
			errorMessage = "Cooling down";
			return false;
		}

		if (!Music.Singleton.IsTimeOpen(CastTimings))
		{
			errorMessage = "Bad timing";
			return false;
		}

		return true;
	}

	// HoldRelease cast button is pressed down
	public void CastBegin()
	{
		IsCasting = true;
		CastStartedAt = Music.Singleton.GetNearestBeatIndex();
		SignalBus.GetInstance(this).EmitSignal(SignalBus.SignalName.CastStarted, this);
	}

	// Instant cast press, or HoldRelease on button release at the right timing
	public virtual void CastPerform(BaseUnit targetUnit)
	{
		SignalBus.GetInstance(this).EmitSignal(SignalBus.SignalName.CastPerformed, this);
		CastPerformInternal(targetUnit);
	}

	// HoldRelease on button release at the bad timing
	public virtual void CastFail(BaseUnit targetUnit)
	{
		SignalBus.GetInstance(this).EmitSignal(SignalBus.SignalName.CastFailed, this);

		if (CastOnFail)
			CastPerformInternal(targetUnit);
	}

	private void CastPerformInternal(BaseUnit targetUnit)
	{
		IsCasting = false;
		if (RecastTime > 0)
			RecastTimerHandle.Start(RecastTime);

		if (TargetType == CastTargetType.None)
			CastOnNone();

		if (TargetType == CastTargetType.Unit)
			CastOnUnit(targetUnit);
	}

	protected virtual void CastOnNone() { }
	protected virtual void CastOnUnit(BaseUnit target) { }
}