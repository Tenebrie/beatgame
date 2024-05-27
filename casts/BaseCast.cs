using System;
using System.Collections.Generic;
using Godot;
namespace Project;

public partial class BaseCast : Node
{
	public class CastSettings
	{
		public CastInputType InputType = CastInputType.Instant;
		public float HoldTime = 1; // beat
		public CastTargetType TargetType = CastTargetType.None;
		public List<UnitAlliance> TargetAlliances = new() { UnitAlliance.Player, UnitAlliance.Neutral, UnitAlliance.Hostile };
		public BeatTime CastTimings = BeatTime.One;
		public BeatTime ReleaseTimings = BeatTime.All;
		public float RecastTime = .1f;
		public bool CastOnFailedRelease = true;
	}

	protected class CastFlags
	{
		public bool CastSuccessful;
	};

	public Timer RecastTimerHandle;
	public double CastStartedAt; // beat index
	public bool IsCasting;

	public CastSettings Settings;

	protected CastFlags Flags = new();

	public readonly BaseUnit Parent;

	public BaseCast(BaseUnit parent)
	{
		Parent = parent;
	}

	public override void _EnterTree()
	{
		Music.Singleton.BeatTick += OnBeatTick;
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

	private void OnBeatTick(BeatTime time)
	{
		if (Settings.InputType != CastInputType.AutoRelease || !IsCasting || Music.Singleton.GetNearestBeatIndex() != CastStartedAt + Settings.HoldTime)
			return;

		CastPerform();
	}

	public bool ValidateTarget(BaseUnit target, out string errorMessage)
	{
		errorMessage = null;

		if (Settings.TargetType == CastTargetType.HostileUnit && target == null)
		{
			errorMessage = "Needs a target";
			return false;
		}
		else if (Settings.TargetType == CastTargetType.HostileUnit && !Settings.TargetAlliances.Contains(target.Alliance))
		{
			errorMessage = "Can't target this unit";
			return false;
		}

		return true;
	}

	public bool ValidateCastTiming(out string errorMessage)
	{
		errorMessage = null;

		EnsureTimerExists();
		if (!RecastTimerHandle.IsStopped())
		{
			errorMessage = "Cooling down";
			return false;
		}

		if (!Music.Singleton.IsTimeOpen(Settings.CastTimings))
		{
			errorMessage = "Bad timing";
			return false;
		}

		return true;
	}

	public bool ValidateReleaseTiming()
	{
		var beatIndex = Music.Singleton.GetNearestBeatIndex();
		if (beatIndex != CastStartedAt + Settings.HoldTime)
			return false;

		if (Settings.InputType == CastInputType.HoldRelease && !Music.Singleton.IsTimeOpen(Settings.ReleaseTimings))
			return false;

		return true;
	}

	private CastTargetData CastTargetData;

	// AutoRelease or HoldRelease cast button is pressed down
	public void CastBegin(CastTargetData targetData)
	{
		IsCasting = true;
		CastStartedAt = Music.Singleton.GetNearestBeatIndex();
		CastTargetData = targetData;
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CastStarted, this);
	}

	// Instant cast press, or HoldRelease on button release at the right timing
	public virtual void CastPerform()
	{
		Flags.CastSuccessful = true;
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CastPerformed, this);
		CastPerformInternal();
	}

	// Cast failed or cancelled
	public virtual void CastFail()
	{
		IsCasting = false;
		Flags.CastSuccessful = false;
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CastFailed, this);

		if (Settings.InputType == CastInputType.HoldRelease && Settings.CastOnFailedRelease)
			CastPerformInternal();
	}

	private void CastPerformInternal()
	{
		IsCasting = false;
		if (Settings.RecastTime > 0)
			RecastTimerHandle.Start(Settings.RecastTime);

		if (Settings.TargetType == CastTargetType.None)
			CastOnNone();

		if (Settings.TargetType == CastTargetType.HostileUnit)
			CastOnUnit(CastTargetData.HostileUnit);

		if (Settings.TargetType == CastTargetType.Point)
			CastOnPoint(CastTargetData.TargetPoint);
	}

	protected virtual void CastOnNone()
	{
		throw new NotImplementedException("CastOnNone not implemented on node " + this.Name);
	}
	protected virtual void CastOnUnit(BaseUnit target)
	{
		throw new NotImplementedException("CastOnUnit not implemented on node " + this.Name);
	}
	protected virtual void CastOnPoint(Vector3 point)
	{
		throw new NotImplementedException("CastOnPoint not implemented on node " + this.Name);
	}
}