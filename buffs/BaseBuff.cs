using System;
using Godot;

namespace Project;

public abstract partial class BaseBuff : Node
{
	public class BuffSettings
	{
		public string FriendlyName = "Unnamed Buff";
		public string Description = null;
		public Func<string> DynamicDesc = null;
		public string IconPath = "res://assets/ui/icon-skill-passive-placeholder.png";
		public bool TicksOnBeat = false;
		public bool RefreshOthersWhenAdded = false;
		public int MaximumStacks = 999;
		public bool Hidden = false;
	}

	public BuffSettings Settings = new();

	public BaseUnit Parent;
	public BaseCast SourceCast;
	public Flag Flags = 0;

	public float CreatedAt = -1;
	public float ExpiresAt = -1;

	public BaseBuff()
	{
		CreatedAt = CastUtils.GetTicksSec();
	}

	public static string MakeDescription(params string[] strings) => CastUtils.MakeDescription(strings);

	float defaultDuration;
	public float Duration
	{
		set
		{
			float time = CastUtils.GetTicksSec();
			ExpiresAt = time + value * Music.Singleton.SecondsPerBeat;
			defaultDuration = value;
		}
	}

	public int Stacks
	{
		get => Parent?.Buffs.Stacks(GetType()) ?? 1;
	}

	public void RefreshDuration()
	{
		Duration = defaultDuration;
	}

	public override void _Process(double delta)
	{
		if (ExpiresAt == -1)
			return;

		float time = CastUtils.GetTicksSec();
		if (time >= ExpiresAt)
		{
			OnDurationExpired();
			Parent.Buffs.Remove(this);
		}
	}

	public virtual void OnDurationExpired() { }
	public virtual void ModifyUnit(BuffUnitStatsVisitor unit) { }
	public virtual void ModifyIncomingDamage(BuffIncomingDamageVisitor damage) { }
	public virtual void ModifyOutgoingDamage(BuffOutgoingDamageVisitor damage) { }
	public virtual void ModifyIncomingRestoration(BuffIncomingRestorationVisitor restoration) { }
	public virtual void ModifyOutgoingRestoration(BuffOutgoingRestorationVisitor restoration) { }
	public virtual void ReactToIncomingDamage(BuffIncomingDamageVisitor damage) { }
	public virtual void ReactToOutgoingDamage(BuffOutgoingDamageVisitor damage) { }
	public virtual void ReactToIncomingRestoration(BuffIncomingRestorationVisitor restoration) { }
	public virtual void ReactToOutgoingRestoration(BuffOutgoingRestorationVisitor restoration) { }
	/// <summary>
	/// Only called once per frame on a representing buff, not on every instance
	/// </summary>
	public virtual void OnBeatTick(BeatTime time, int stacks) { }

	public enum Flag : int
	{
		SkillCreated = 1,
	}
}
