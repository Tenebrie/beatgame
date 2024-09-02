using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;
namespace Project;

public partial class BaseCast : Node
{
	public const float GlobalCooldownDuration = 1f;

	[Signal] public delegate void StartedEventHandler();
	[Signal] public delegate void PreparedEventHandler();
	[Signal] public delegate void CompletedEventHandler();
	[Signal] public delegate void InterruptedEventHandler();
	[Signal] public delegate void FailedEventHandler();

	public class CastSettings
	{
		public string FriendlyName = "Unnamed Spell";
		public string Description = null;
		public string LoreDescription = null;
		public string IconPath = "res://assets/ui/icon-skill-active-placeholder.png";
		public float HoldTime = 1; // beat
		public CastInputType InputType = CastInputType.Instant;
		public CastTargetType TargetType = CastTargetType.None;
		public CastTickMode TickMode = CastTickMode.WhileCasting;
		public float TickDuration = 1; // beats
		public Dictionary<ObjectResourceType, float> ResourceCost = ObjectResource.MakeDictionary(0f);
		public Dictionary<ObjectResourceType, float> ResourceCostPerBeat = ObjectResource.MakeDictionary(0f);
		public int Charges = 1;
		public float RecastTime = 0;
		public GlobalCooldownMode GlobalCooldown = GlobalCooldownMode.Trigger | GlobalCooldownMode.Receive;
		public bool CooldownOnCancel = true;
		public bool ReversedCastBar = false;
		public bool HiddenCastBar = false;
		public float MaximumRange = Mathf.Inf;

		/// <summary>Only for CastInputType.AutoRelease</summary>
		public float PrepareTime = 0; // beats
		/// <summary>Only for CastInputType.AutoRelease</summary>
		public bool TickWhilePreparing = false;
	}

	protected class CastFlags
	{
		public bool CastSuccessful;
	};

	Timer CastPrepareTimer;
	Timer CastCompleteTimer;
	Timer CastChannelTickTimer;
	Timer GlobalCooldownTimerHandle;
	Timer ChargesTimerHandle;
	double LastCooldownDuration = 1;

	public BaseCastAutoAttack AutoAttack;

	public int ChargesRemaining;
	public float CastStartedAt; // engine time
	public bool IsCasting
	{
		get => !CastCompleteTimer.IsStopped();
	}
	public bool IsPreparing
	{
		get => !CastPrepareTimer.IsStopped();
	}

	public CastSettings Settings;

	protected CastFlags Flags = new();

	public readonly BaseUnit Parent;

	public BaseCast(BaseUnit parent)
	{
		Parent = parent;
	}

	public static string MakeDescription(params string[] strings) => CastUtils.MakeDescription(strings);
	public static string GetDescription(CastSettings castSettings)
	{
		StringBuilder builder = new();
		// Description
		builder.Append(castSettings.Description).Append('\n');

		// Resource cost
		if (castSettings.ResourceCost[ObjectResourceType.Health] > 0)
			builder.Append($"\n[color={Colors.Health}]Health cost:[/color] ").Append(castSettings.ResourceCost[ObjectResourceType.Health]);
		if (castSettings.ResourceCost[ObjectResourceType.Mana] > 0)
			builder.Append($"\n[color={Colors.Mana}]Mana cost:[/color] ").Append(castSettings.ResourceCost[ObjectResourceType.Mana]);

		// Resource cost per beat
		if (castSettings.ResourceCostPerBeat[ObjectResourceType.Health] > 0)
			builder.Append($"\n[color={Colors.Health}]Channeling Health cost:[/color] ").Append(castSettings.ResourceCostPerBeat[ObjectResourceType.Health]).Append(" / beat");
		if (castSettings.ResourceCostPerBeat[ObjectResourceType.Mana] > 0)
			builder.Append($"\n[color={Colors.Mana}]Channeling Mana cost:[/color] ").Append(castSettings.ResourceCostPerBeat[ObjectResourceType.Mana]).Append(" / beat");

		// Input type
		builder.Append($"\n[color={Colors.Passive}]Input:[/color] ");
		if (castSettings.InputType == CastInputType.Instant)
			builder.Append("Instant");
		else if (castSettings.InputType == CastInputType.AutoRelease)
			builder.Append($"Cast ({castSettings.HoldTime} beat{(castSettings.HoldTime == 1 ? "" : "s")})");

		// Charges
		if (castSettings.Charges > 1)
			builder.Append($"\n[color={Colors.Passive}]Charges:[/color] ").Append($"{castSettings.Charges}");

		// Recast time
		if (castSettings.RecastTime >= 0.25f)
			builder.Append($"\n[color={Colors.Passive}]Cooldown:[/color] ").Append($"{castSettings.RecastTime} beat{(castSettings.RecastTime == 1 ? "" : "s")}");

		// Lore
		if (castSettings.LoreDescription != null)
			builder.Append("\n\n").Append(Colors.Lore(castSettings.LoreDescription));

		return builder.ToString();
	}

	bool settingsPrepared = false;
	public void PrepareSettings()
	{
		settingsPrepared = true;
		Settings.ResourceCost[ObjectResourceType.Mana] = Settings.ResourceCost.GetValueOrDefault(ObjectResourceType.Mana) * (1 - Parent.Buffs.State.CastManaEfficiency);
	}

	public override void _EnterTree()
	{
		if (!settingsPrepared)
			PrepareSettings();
		InitializeTimers();
		ChargesRemaining = Settings.Charges;

		AutoAttack = new BaseCastAutoAttack(this);
		AddChild(AutoAttack);
	}

	private void InitializeTimers()
	{
		if (ChargesTimerHandle != null && GlobalCooldownTimerHandle != null)
			return;

		CastPrepareTimer = new Timer { OneShot = true };
		CastPrepareTimer.Timeout += CastPrepare;
		AddChild(CastPrepareTimer);

		CastCompleteTimer = new Timer { OneShot = true };
		CastCompleteTimer.Timeout += () => CastComplete();
		AddChild(CastCompleteTimer);

		CastChannelTickTimer = new Timer();
		CastChannelTickTimer.Timeout += CastChannelTick;
		AddChild(CastChannelTickTimer);

		if (Settings.TickMode is CastTickMode.AlwaysResetOnCast or CastTickMode.AlwaysIndependent)
			CastChannelTickTimer.Start(Settings.TickDuration * Music.GetSecondsPerBeat());

		ChargesTimerHandle = new Timer { OneShot = true };
		ChargesTimerHandle.Timeout += () =>
		{
			ChargesRemaining += 1;
			if (ChargesRemaining < Settings.Charges)
				ChargesTimerHandle.Start(Settings.RecastTime * Music.Singleton.SecondsPerBeat);
		};
		AddChild(ChargesTimerHandle);

		GlobalCooldownTimerHandle = new Timer { OneShot = true };
		AddChild(GlobalCooldownTimerHandle);
		GlobalCooldownTimerHandle.WaitTime = GlobalCooldownDuration * Music.Singleton.SecondsPerBeat;
	}

	public bool ValidateTarget(CastTargetData target, out string errorMessage)
	{
		errorMessage = null;

		if ((Settings.TargetType & CastTargetType.AlliedUnit) > 0 && (target.AlliedUnit == null || target.AlliedUnit is PlayerController))
		{
			errorMessage = "Needs an allied target";
			return false;
		}
		if ((Settings.TargetType & CastTargetType.HostileUnit) > 0 && target.HostileUnit == null)
		{
			errorMessage = "Needs a hostile target";
			return false;
		}

		if (double.IsFinite(Settings.MaximumRange))
		{
			Vector3? targetPoint = null;
			float selectionDistance = 0;
			if ((Settings.TargetType & CastTargetType.AlliedUnit) > 0)
			{
				targetPoint = target.AlliedUnit.GlobalCastAimPosition;
				selectionDistance = target.AlliedUnit.Targetable.selectionRadius;
			}
			else if ((Settings.TargetType & CastTargetType.HostileUnit) > 0)
			{
				targetPoint = target.HostileUnit.GlobalCastAimPosition;
				selectionDistance = target.HostileUnit.Targetable.selectionRadius;
			}

			float dist = Parent.GlobalCastAimPosition.DistanceTo(targetPoint ?? Vector3.Zero);
			if (targetPoint != null && dist > Settings.MaximumRange + selectionDistance + Parent.Targetable.selectionRadius)
			{
				errorMessage = "Out of range";
				return false;
			}
		}

		return true;
	}

	public enum CastQueueMode { Instant, Queue, None };
	public CastQueueMode ValidateIfCastIsPossible(CastTargetData target, out string errorMessage)
	{
		if (!ValidateTarget(target, out errorMessage))
		{
			return CastQueueMode.None;
		}

		else if (Parent.Health.Current <= Settings.ResourceCost[ObjectResourceType.Health])
		{
			errorMessage = "Not enough health";
			return CastQueueMode.Queue;
		}
		else if (Parent.Mana.Current < 0 && Settings.ResourceCost[ObjectResourceType.Mana] > 0)
		{
			errorMessage = "Not enough mana";
			return CastQueueMode.Queue;
		}

		else if (ChargesRemaining <= 0 && ChargesTimerHandle.TimeLeft > Music.Singleton.QueueingWindow)
		{
			errorMessage = "No charges available";
			return CastQueueMode.None;
		}
		else if (GlobalCooldownTimerHandle.TimeLeft > Music.Singleton.QueueingWindow)
		{
			errorMessage = "Global cooldown";
			return CastQueueMode.None;
		}
		else if (ChargesRemaining <= 0 && ChargesTimerHandle.TimeLeft > Music.Singleton.TimingWindow)
		{
			errorMessage = "No charges available";
			return CastQueueMode.Queue;
		}
		else if (GlobalCooldownTimerHandle.TimeLeft > Music.Singleton.TimingWindow)
		{
			errorMessage = "Global cooldown";
			return CastQueueMode.Queue;
		}

		return CastQueueMode.Instant;
	}

	public bool ValidateReleaseTiming(out float timeOffset)
	{
		var time = CastUtils.GetEngineTime();
		var targetTime = CastStartedAt + Settings.HoldTime * Music.GetSecondsPerBeat();
		timeOffset = targetTime - time;
		if (Math.Abs(timeOffset) > Music.Singleton.TimingWindow)
			return false;

		return true;
	}

	private CastTargetData CastTargetData;

	public void CastBegin(CastTargetData targetData)
	{
		float timeGraceGiven = GetCooldownTimeGrace();
		CastBegin(targetData, timeGraceGiven);
	}

	public void CastBegin(CastTargetData targetData, float timeGraceGiven)
	{
		Parent.Health.Damage(Settings.ResourceCost[ObjectResourceType.Health], this);
		Parent.Mana.Damage(Settings.ResourceCost[ObjectResourceType.Mana], this);
		CastStartedAt = CastUtils.GetEngineTime();
		CastTargetData = targetData;
		OnCastStarted(targetData);
		if (Settings.InputType == CastInputType.Instant || (Settings.InputType == CastInputType.AutoRelease && Settings.HoldTime == 0))
			CastComplete(timeGraceGiven);
		else
		{
			if (Settings.PrepareTime > 0)
				CastPrepareTimer.Start(Settings.PrepareTime * Music.GetSecondsPerBeat() + timeGraceGiven);

			CastCompleteTimer.Start((Settings.PrepareTime + Settings.HoldTime) * Music.GetSecondsPerBeat() + timeGraceGiven);

			if (Settings.TickMode is CastTickMode.AlwaysResetOnCast)
				CastChannelTickTimer.Stop();
			if (Settings.TickMode is CastTickMode.WhileCasting or CastTickMode.AlwaysResetOnCast)
				CastChannelTickTimer.Start(Settings.TickDuration * Music.GetSecondsPerBeat());
		}

		EmitSignal(SignalName.Started);
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CastStarted, this);
	}

	public void CastPrepare()
	{
		OnPrepCompleted(CastTargetData);

		EmitSignal(SignalName.Prepared);
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CastPrepared, this);
	}

	public void CastComplete(float timeAdjustment = 0)
	{
		StartCooldown(timeAdjustment);
		Flags.CastSuccessful = true;
		StopCastTimers();
		OnCastCompleted(CastTargetData);
		OnCastCompletedOrFailed(CastTargetData);

		EmitSignal(SignalName.Completed);
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CastCompleted, this);
	}

	public void CastChannelTick()
	{
		// Resource cost per beat
		if (Settings.ResourceCostPerBeat[ObjectResourceType.Health] > 0)
		{
			var damage = Settings.ResourceCostPerBeat[ObjectResourceType.Health] * (float)Music.MinBeatSize;
			if (Parent.Health.Current > damage)
				Parent.Health.DamageSilently(damage, Parent, this, SilentDamageReason.ResourceCost);
			else
				CastFail();
		}
		if (Settings.ResourceCostPerBeat[ObjectResourceType.Mana] > 0)
		{
			var damage = Settings.ResourceCostPerBeat[ObjectResourceType.Mana] * (float)Music.MinBeatSize;
			if (Parent.Mana.Current > 0)
				Parent.Mana.DamageSilently(damage, Parent, this, SilentDamageReason.ResourceCost);
			else
				CastFail();
		}

		OnCastTicked(CastTargetData);
	}

	public void CastFail()
	{
		if (Settings.CooldownOnCancel)
			StartCooldown();
		Flags.CastSuccessful = false;

		StopCastTimers();
		OnCastFailed(CastTargetData);
		OnCastCompletedOrFailed(CastTargetData);

		EmitSignal(SignalName.Failed);
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CastFailed, this);
	}

	public void CastInterrupt()
	{
		if (Settings.CooldownOnCancel)
			StartCooldown();
		Flags.CastSuccessful = false;

		StopCastTimers();
		OnCastFailed(CastTargetData);
		OnCastCompletedOrFailed(CastTargetData);

		EmitSignal(SignalName.Interrupted);
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CastInterrupted, this);
	}

	private void StopCastTimers()
	{
		CastPrepareTimer.Stop();
		CastCompleteTimer.Stop();
		if (Settings.TickMode is CastTickMode.WhileCasting)
			CastChannelTickTimer.Stop();
	}

	public void StartCooldown(float timeAdjustment = 0)
	{
		if (Settings.GlobalCooldown.Triggers() && Parent is PlayerController player)
			player.Spellcasting.TriggerGlobalCooldown(timeAdjustment);
		if (Settings.RecastTime > 0)
		{
			ChargesRemaining -= 1;

			ChargesTimerHandle.Stop();
			ChargesTimerHandle.Start(Settings.RecastTime * Music.Singleton.SecondsPerBeat + timeAdjustment);
			if (ChargesTimerHandle.WaitTime > GlobalCooldownTimerHandle.TimeLeft)
				LastCooldownDuration = ChargesTimerHandle.WaitTime;
		}
	}

	public void StartGlobalCooldown(float timeAdjustment = 0)
	{
		GlobalCooldownTimerHandle.Start(GlobalCooldownDuration * Music.Singleton.SecondsPerBeat + timeAdjustment);
		if (GlobalCooldownTimerHandle.WaitTime > ChargesTimerHandle.TimeLeft)
			LastCooldownDuration = GlobalCooldownTimerHandle.WaitTime;
	}

	public float GetCooldownTimeLeft()
	{
		if (ChargesRemaining > 0)
			return (float)GlobalCooldownTimerHandle.TimeLeft;

		return (float)Math.Max(ChargesTimerHandle.TimeLeft, GlobalCooldownTimerHandle.TimeLeft);
	}

	public float GetCooldownWaitTime()
	{
		return (float)LastCooldownDuration;
	}

	/// <summary>
	/// The player is allowed to skip the last 100ms (TimingWindow) of cast cooldown.
	/// The skipped time is then added to the cast time.
	/// </summary>
	float GetCooldownTimeGrace()
	{
		if (ChargesRemaining > 0)
			return (float)GlobalCooldownTimerHandle.TimeLeft;

		return (float)Math.Max(GlobalCooldownTimerHandle.TimeLeft, ChargesTimerHandle.TimeLeft);
	}

	protected virtual void OnCastStarted(CastTargetData _) { }
	protected virtual void OnCastTicked(CastTargetData _) { }
	protected virtual void OnPrepCompleted(CastTargetData _) { }
	protected virtual void OnCastCompleted(CastTargetData _) { }
	protected virtual void OnCastFailed(CastTargetData _) { }
	protected virtual void OnCastCompletedOrFailed(CastTargetData _) { }
}