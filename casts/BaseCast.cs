using System;
using System.Collections.Generic;
using Godot;
namespace Project;

public partial class BaseCast : Node
{
	public class CastSettings
	{
		public string FriendlyName = "Unnamed Spell";
		public float HoldTime = 1; // beat
		public CastInputType InputType = CastInputType.Instant;
		public CastTargetType TargetType = CastTargetType.None;
		public BeatTime CastTimings = BeatTime.Quarter;
		public BeatTime ChannelingTickTimings = 0;
		public float RecastTime = .1f;
		public bool ReversedCastBar = false;
		public bool HiddenCastBar = false;

		/// <summary>Only for CastInputType.AutoRelease</summary>
		public float PrepareTime = 0; // beats
		/// <summary>Only for CastInputType.AutoRelease</summary>
		public bool TickWhilePreparing = false;

		/// <summary>Only for CastInputType.HoldRelease</summary>
		public BeatTime ReleaseTimings = BeatTime.All;
		/// <summary>Only for CastInputType.HoldRelease</summary>
		public bool CastOnFailedRelease = true;
	}

	protected class CastFlags
	{
		public bool CastSuccessful;
	};

	public Timer RecastTimerHandle;
	public double CastStartedAt; // beat index
	public bool IsCasting;
	public bool IsPreparing;

	public CastSettings Settings;

	protected CastFlags Flags = new();

	public readonly BaseUnit Parent;
	public BaseUnit Unit { get => Parent; }

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
		if (RecastTimerHandle != null)
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

		var nearestBeatIndex = Music.Singleton.GetNearestBeatIndex();
		if ((time & Settings.ChannelingTickTimings) > 0 && (Settings.TickWhilePreparing || nearestBeatIndex > CastStartedAt + Settings.PrepareTime))
			OnCastTicked(CastTargetData, time);

		if (Settings.PrepareTime > 0 && nearestBeatIndex == CastStartedAt + Settings.PrepareTime)
			CastPrepare();

		if (Settings.InputType == CastInputType.AutoRelease && nearestBeatIndex == CastStartedAt + Settings.PrepareTime + Settings.HoldTime)
			CastPerform();
	}

	public bool ValidateTarget(CastTargetData target, out string errorMessage)
	{
		errorMessage = null;

		if ((Settings.TargetType & CastTargetType.HostileUnit) > 0 && target.HostileUnit == null)
		{
			errorMessage = "Needs a hostile target";
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
		if (Settings.PrepareTime > 0)
			IsPreparing = true;
		CastStartedAt = Music.Singleton.GetNearestBeatIndex();
		CastTargetData = targetData;
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CastStarted, this);
		OnCastStarted(targetData);
		if (Settings.InputType == CastInputType.Instant || (Settings.InputType == CastInputType.AutoRelease && Settings.HoldTime == 0))
			CastPerform();
	}

	public void CastPrepare()
	{
		IsPreparing = false;
		OnPrepCompleted(CastTargetData);
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
	protected virtual void OnCastTicked(CastTargetData _, BeatTime time) { }
	protected virtual void OnPrepCompleted(CastTargetData _) { }
	protected virtual void OnCastCompleted(CastTargetData _) { }
}