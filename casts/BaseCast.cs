using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;
namespace Project;

public partial class BaseCast : Node
{
	public const float GlobalCooldownDuration = 2f;

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
		public BeatTime castTimings = BeatTime.Free;
		public BeatTime CastTimings
		{
			get => Preferences.Singleton.ChillMode ? BeatTime.Free : castTimings;
			set => castTimings = value;
		}
		public BeatTime ChannelingTickTimings = 0;
		public bool ChannelWhileNotCasting = false;
		public Dictionary<ObjectResourceType, float> ResourceCost = ObjectResource.MakeDictionary(0f);
		public Dictionary<ObjectResourceType, float> ResourceCostPerBeat = ObjectResource.MakeDictionary(0f);
		public int Charges = 1;
		public float RecastTime = 0;
		public bool GlobalCooldown = true;
		public bool CooldownOnCancel = true;
		public bool ReversedCastBar = false;
		public bool HiddenCastBar = false;
		public float MaximumRange = Mathf.Inf;

		/// <summary>Only for CastInputType.AutoRelease</summary>
		public float PrepareTime = 0; // beats
		/// <summary>Only for CastInputType.AutoRelease</summary>
		public bool TickWhilePreparing = false;

		/// <summary>Only for CastInputType.HoldRelease</summary>
		public bool CastOnFailedRelease = false;
	}

	protected class CastFlags
	{
		public bool CastSuccessful;
	};

	Timer GlobalCooldownTimerHandle;
	Timer ChargesTimerHandle;
	public int ChargesRemaining;
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
		else if (castSettings.InputType == CastInputType.HoldRelease)
			builder.Append($"Hold ({castSettings.HoldTime} beat{(castSettings.HoldTime == 1 ? "" : "s")})");

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
		EnsureTimerExists();
		Music.Singleton.BeatTick += OnBeatTick;
	}

	public override void _ExitTree()
	{
		Music.Singleton.BeatTick -= OnBeatTick;
		if (GlobalCooldownTimerHandle != null)
			RemoveChild(GlobalCooldownTimerHandle);
		if (ChargesTimerHandle != null)
			RemoveChild(ChargesTimerHandle);
	}

	private void EnsureTimerExists()
	{
		if (ChargesTimerHandle != null && GlobalCooldownTimerHandle != null)
			return;

		ChargesRemaining = Settings.Charges;
		ChargesTimerHandle = new Timer { OneShot = true };
		GlobalCooldownTimerHandle = new Timer { OneShot = true };

		ChargesTimerHandle.Timeout += () =>
		{
			ChargesRemaining += 1;
			if (ChargesRemaining < Settings.Charges)
				ChargesTimerHandle.Start(Settings.RecastTime * Music.Singleton.SecondsPerBeat);
		};
		AddChild(ChargesTimerHandle);
		AddChild(GlobalCooldownTimerHandle);
	}

	private void OnBeatTick(BeatTime time)
	{
		if (!IsCasting && !Settings.ChannelWhileNotCasting)
			return;

		var beatIndex = Music.Singleton.BeatIndex;
		if ((time & Settings.ChannelingTickTimings) > 0 && (
				Settings.ChannelWhileNotCasting ||
				Settings.TickWhilePreparing ||
				Music.Singleton.BeatIndex > CastStartedAt + Settings.PrepareTime)
			)
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

	public bool ValidateIfCastIsPossible(CastTargetData target, out string errorMessage)
	{
		if (!Music.Singleton.IsPlaying)
		{
			errorMessage = "Song has not started yet";
			return false;
		}
		if (ChargesRemaining == 0 && ChargesTimerHandle.TimeLeft > Music.Singleton.TimingWindow)
		{
			errorMessage = "No charges available";
			return false;
		}
		if (GlobalCooldownTimerHandle.TimeLeft > Music.Singleton.TimingWindow)
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
		if (Parent.Mana.Current < 0 && Settings.ResourceCost[ObjectResourceType.Mana] > 0)
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
		OnCastStarted(targetData);
		if (Settings.InputType == CastInputType.Instant || (Settings.InputType == CastInputType.AutoRelease && Settings.HoldTime == 0))
			CastComplete();

		EmitSignal(SignalName.Started);
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CastStarted, this);
	}

	public void CastPrepare()
	{
		IsPreparing = false;
		OnPrepCompleted(CastTargetData);

		EmitSignal(SignalName.Prepared);
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CastPrepared, this);
	}

	public void CastComplete()
	{
		IsCasting = false;
		StartCooldown();
		Flags.CastSuccessful = true;
		OnCastCompleted(CastTargetData);
		OnCastCompletedOrFailed(CastTargetData);

		EmitSignal(SignalName.Completed);
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CastCompleted, this);
	}

	public void CastFail()
	{
		IsCasting = false;
		if (Settings.CooldownOnCancel)
			StartCooldown();
		Flags.CastSuccessful = false;

		OnCastFailed(CastTargetData);
		OnCastCompletedOrFailed(CastTargetData);
		if (Settings.InputType == CastInputType.HoldRelease && Settings.CastOnFailedRelease)
		{
			OnCastCompleted(CastTargetData);
			OnCastCompletedOrFailed(CastTargetData);
		}

		EmitSignal(SignalName.Failed);
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CastFailed, this);
	}

	public void CastInterrupt()
	{
		IsCasting = false;
		if (Settings.CooldownOnCancel)
			StartCooldown();
		Flags.CastSuccessful = false;

		OnCastFailed(CastTargetData);
		OnCastCompletedOrFailed(CastTargetData);

		EmitSignal(SignalName.Interrupted);
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CastInterrupted, this);
	}

	public void StartCooldown()
	{
		if (Settings.GlobalCooldown && Parent is PlayerController player)
			player.Spellcasting.TriggerGlobalCooldown();
		if (Settings.RecastTime > 0)
		{
			ChargesRemaining -= 1;
			if (ChargesTimerHandle.IsStopped())
				ChargesTimerHandle.Start(Settings.RecastTime * Music.Singleton.SecondsPerBeat);
		}
	}

	public void StartGlobalCooldown()
	{
		GlobalCooldownTimerHandle.Start(GlobalCooldownDuration * Music.Singleton.SecondsPerBeat);
	}

	public float GetCooldownTimeLeft()
	{
		if (ChargesRemaining > 0)
			return (float)GlobalCooldownTimerHandle.TimeLeft;

		return (float)Math.Max(ChargesTimerHandle.TimeLeft, GlobalCooldownTimerHandle.TimeLeft);
	}

	public float GetCooldownWaitTime()
	{
		if (!Settings.GlobalCooldown)
			return (float)ChargesTimerHandle.WaitTime;
		return (float)Math.Max(ChargesTimerHandle.WaitTime, GlobalCooldownTimerHandle.WaitTime);
	}

	protected virtual void OnCastStarted(CastTargetData _) { }
	protected virtual void OnCastTicked(CastTargetData _, BeatTime time) { }
	protected virtual void OnPrepCompleted(CastTargetData _) { }
	protected virtual void OnCastCompleted(CastTargetData _) { }
	protected virtual void OnCastFailed(CastTargetData _) { }
	protected virtual void OnCastCompletedOrFailed(CastTargetData _) { }
}