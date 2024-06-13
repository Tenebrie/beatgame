using System;
using System.Collections.Generic;
using Godot;
namespace Project;

public partial class BaseCast : Node
{
	public class CastSettings
	{
		public string FriendlyName = "Unnamed Spell";
		public string Description = "No description";
		public string LoreDescription = null;
		public string IconPath = "res://assets/ui/icon-skill-active-placeholder.png";
		public float HoldTime = 1; // beat
		public CastInputType InputType = CastInputType.Instant;
		public CastTargetType TargetType = CastTargetType.None;
		public BeatTime castTimings = BeatTime.Free;
		public BeatTime CastTimings
		{
			get => Preferences.Singleton.ChillMode ? BeatTime.Free : castTimings;
			set => castTimings = value;
		}
		public BeatTime ChannelingTickTimings = 0;
		public Dictionary<ObjectResourceType, float> ResourceCost = ObjectResource.MakeDictionary(0f);
		public Dictionary<ObjectResourceType, float> ResourceCostPerBeat = ObjectResource.MakeDictionary(0f);
		public int Charges = 1;
		public float RecastTime = .1f;
		public bool ReversedCastBar = false;
		public bool HiddenCastBar = false;

		/// <summary>Only for CastInputType.AutoRelease</summary>
		public float PrepareTime = 0; // beats
		/// <summary>Only for CastInputType.AutoRelease</summary>
		public bool TickWhilePreparing = false;

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

	public BaseCast(BaseUnit parent)
	{
		Parent = parent;
	}

	public static string MakeDescription(params string[] strings) => CastUtils.MakeDescription(strings);

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

		var beatIndex = Music.Singleton.BeatIndex;
		if ((time & Settings.ChannelingTickTimings) > 0 && (Settings.TickWhilePreparing || Music.Singleton.BeatIndex > CastStartedAt + Settings.PrepareTime))
			OnCastTicked(CastTargetData, time);

		if (!IsCasting)
			return;

		if (Settings.PrepareTime > 0 && IsPreparing && beatIndex == CastStartedAt + Settings.PrepareTime)
			CastPrepare();

		if (!IsCasting)
			return;

		if (Settings.InputType == CastInputType.AutoRelease && beatIndex == CastStartedAt + Settings.PrepareTime + Settings.HoldTime)
			CastComplete();

		if (!IsCasting)
			return;

		// Resource cost per beat
		if (Settings.ResourceCostPerBeat[ObjectResourceType.Health] > 0)
		{
			var damage = Settings.ResourceCostPerBeat[ObjectResourceType.Health] * (float)Music.MinBeatSize;
			if (Parent.Health.Current > damage)
				Parent.Health.Damage(damage, this);
			else
				CastFail();
		}
		if (Settings.ResourceCostPerBeat[ObjectResourceType.Mana] > 0)
		{
			var damage = Settings.ResourceCostPerBeat[ObjectResourceType.Mana] * (float)Music.MinBeatSize;
			if (Parent.Mana.Current > 0)
				Parent.Mana.Damage(damage, this);
			else
				CastFail();
		}
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

	public bool ValidateIfCastIsPossible(CastTargetData target, out string errorMessage)
	{
		EnsureTimerExists();
		if (RecastTimerHandle.TimeLeft > Music.Singleton.TimingWindow)
		{
			errorMessage = "Cooling down";
			return false;
		}

		if (!ValidateTarget(target, out errorMessage))
		{
			return false;
		}

		if (Parent.Health.Current <= Settings.ResourceCost[ObjectResourceType.Health])
		{
			errorMessage = "Not enough health";
			return false;
		}
		if (Parent.Mana.Current < 0)
		{
			errorMessage = "Not enough mana";
			return false;
		}

		var beatOffset = Math.Abs(Music.Singleton.GetCurrentBeatOffset(Settings.CastTimings));
		if (!Preferences.Singleton.ChillMode && beatOffset > Music.Singleton.TimingWindowMs)
		{
			errorMessage = "Bad timing";
			return false;
		}

		return true;
	}

	public bool ValidateReleaseTiming()
	{
		var time = Music.Singleton.SongTime;
		var targetTime = CastStartedAt * Music.Singleton.SecondsPerBeat * 1000 + Settings.HoldTime * Music.Singleton.SecondsPerBeat * 1000;
		if (Math.Abs(time - targetTime) > Music.Singleton.TimingWindowMs)
			return false;

		return true;
	}

	private CastTargetData CastTargetData;

	public void CastBegin(CastTargetData targetData)
	{
		IsCasting = true;
		if (Settings.PrepareTime > 0)
			IsPreparing = true;

		Parent.Health.Damage(Settings.ResourceCost[ObjectResourceType.Health], this);
		Parent.Mana.Damage(Settings.ResourceCost[ObjectResourceType.Mana], this);
		CastStartedAt = Music.Singleton.GetNearestBeatIndex(Settings.CastTimings);
		CastTargetData = targetData;
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CastStarted, this);
		OnCastStarted(targetData);
		if (Settings.InputType == CastInputType.Instant || (Settings.InputType == CastInputType.AutoRelease && Settings.HoldTime == 0))
			CastComplete();
	}

	public void CastPrepare()
	{
		IsPreparing = false;
		OnPrepCompleted(CastTargetData);
	}

	public void CastComplete()
	{
		Flags.CastSuccessful = true;
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CastPerformed, this);
		CastCompleteInternal();
	}

	public void CastFail()
	{
		IsCasting = false;
		Flags.CastSuccessful = false;
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CastFailed, this);

		OnCastFailed(CastTargetData);
		OnCastCompletedOrFailed(CastTargetData);
		if (Settings.InputType == CastInputType.HoldRelease && Settings.CastOnFailedRelease)
			CastCompleteInternal();
	}

	private void CastCompleteInternal()
	{
		IsCasting = false;
		StartCooldown();

		OnCastCompleted(CastTargetData);
		OnCastCompletedOrFailed(CastTargetData);
	}

	public void StartCooldown()
	{
		EnsureTimerExists();
		if (Settings.RecastTime > 0)
			RecastTimerHandle.Start(Settings.RecastTime * Music.Singleton.SecondsPerBeat);
	}

	protected virtual void OnCastStarted(CastTargetData _) { }
	protected virtual void OnCastTicked(CastTargetData _, BeatTime time) { }
	protected virtual void OnPrepCompleted(CastTargetData _) { }
	protected virtual void OnCastCompleted(CastTargetData _) { }
	protected virtual void OnCastFailed(CastTargetData _) { }
	protected virtual void OnCastCompletedOrFailed(CastTargetData _) { }
}