using Godot;

namespace Project;

public abstract partial class BaseBuff : Node
{
	public class BuffSettings
	{
		public string FriendlyName = "Unnamed Buff";
		public string Description = "No description";
		public string IconPath = "res://assets/ui/icon-skill-passive-placeholder.png";
		public bool TicksOnBeat = false;
		public bool RefreshOthersWhenAdded = false;
		public int MaximumStacks = 100;
	}

	public BuffSettings Settings = new();

	public BaseUnit Parent;
	public BaseCast SourceCast;
	public Flag Flags = 0;

	public float CreatedAt = -1;
	public float ExpiresAt = -1;

	public BaseBuff()
	{
		CreatedAt = Time.GetTicksMsec();
	}

	public static string MakeDescription(params string[] strings) => CastUtils.MakeDescription(strings);

	float defaultDuration;
	public float Duration
	{
		set
		{
			float time = Time.GetTicksMsec();
			ExpiresAt = time + value * Music.Singleton.SecondsPerBeat * 1000;
			defaultDuration = value;
		}
	}

	public void RefreshDuration()
	{
		Duration = defaultDuration;
	}

	public override void _Process(double delta)
	{
		if (ExpiresAt == -1)
			return;

		float time = Time.GetTicksMsec();
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
