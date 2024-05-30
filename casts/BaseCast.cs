using System;
using System.Collections.Generic;
using Godot;
namespace Project;

public partial class BaseCast : Node
{
	public class CastSettings
	{
		public string FriendlyName;
		public CastInputType InputType = CastInputType.Instant;
		public float HoldTime = 1; // beat
		public CastTargetType TargetType = CastTargetType.None;
		public List<UnitAlliance> TargetAlliances = new() { UnitAlliance.Player, UnitAlliance.Neutral, UnitAlliance.Hostile };
		public BeatTime CastTimings = BeatTime.One;
		public BeatTime ReleaseTimings = BeatTime.All;
		public BeatTime ChannelingTickTimings = 0;
		public float RecastTime = .1f;
		public bool CastOnFailedRelease = true;
	}

	protected BaseCast()
	{
		Settings = new()
		{
			FriendlyName = this.ToString(),
		};
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
		if (!IsCasting)
			return;

		if ((time & Settings.ChannelingTickTimings) > 0)
			OnCastTicked();

		if (Settings.InputType == CastInputType.AutoRelease && Music.Singleton.GetNearestBeatIndex() == CastStartedAt + Settings.HoldTime)
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

	public void CastBegin(CastTargetData targetData)
	{
		IsCasting = true;
		CastStartedAt = Music.Singleton.GetNearestBeatIndex();
		CastTargetData = targetData;
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CastStarted, this);
		OnCastStarted(targetData);
		if (Settings.InputType == CastInputType.Instant || (Settings.InputType == CastInputType.AutoRelease && Settings.HoldTime == 0))
			CastPerform();
	}

	public void CastPerform()
	{
		Flags.CastSuccessful = true;
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CastPerformed, this);
		CastPerformInternal();
	}

	public void CastFail()
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

		OnCastCompleted(CastTargetData);
	}

	protected virtual void OnCastStarted(CastTargetData _) { }
	protected virtual void OnCastTicked() { }
	protected virtual void OnCastCompleted(CastTargetData _) { }
}