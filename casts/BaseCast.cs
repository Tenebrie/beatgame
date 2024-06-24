using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;
namespace Project;

public partial class BaseCast : Node
{
	public const float GlobalCooldownDuration = 2f;

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
		IsCasting = false;
		StartCooldown();
		Flags.CastSuccessful = true;
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CastPerformed, this);
		OnCastCompleted(CastTargetData);
		OnCastCompletedOrFailed(CastTargetData);
	}

	public void CastFail()
	{
		IsCasting = false;
		if (Settings.CooldownOnCancel)
			StartCooldown();
		Flags.CastSuccessful = false;
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CastFailed, this);

		OnCastFailed(CastTargetData);
		OnCastCompletedOrFailed(CastTargetData);
		if (Settings.InputType == CastInputType.HoldRelease && Settings.CastOnFailedRelease)
		{
			OnCastCompleted(CastTargetData);
			OnCastCompletedOrFailed(CastTargetData);
		}
	}

	public void StartCooldown()
	{
		if (Settings.GlobalCooldown && Parent is PlayerController player)
			player.Spellcasting.TriggerGlobalCooldown();
		if (Settings.RecastTime > 0 && RecastTimerHandle.TimeLeft < Settings.RecastTime * Music.Singleton.SecondsPerBeat)
			RecastTimerHandle.Start(Settings.RecastTime * Music.Singleton.SecondsPerBeat);
	}

	public void StartGlobalCooldown()
	{
		if (RecastTimerHandle.TimeLeft < GlobalCooldownDuration * Music.Singleton.SecondsPerBeat)
			RecastTimerHandle.Start(GlobalCooldownDuration * Music.Singleton.SecondsPerBeat);
	}

	protected virtual void OnCastStarted(CastTargetData _) { }
	protected virtual void OnCastTicked(CastTargetData _, BeatTime time) { }
	protected virtual void OnPrepCompleted(CastTargetData _) { }
	protected virtual void OnCastCompleted(CastTargetData _) { }
	protected virtual void OnCastFailed(CastTargetData _) { }
	protected virtual void OnCastCompletedOrFailed(CastTargetData _) { }
}