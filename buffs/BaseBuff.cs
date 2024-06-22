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
		public bool PreventStacking = false;
		public bool RefreshOthersWhenAdded = false;
	}

	public BuffSettings Settings = new();

	public BaseUnit Parent;
	public BaseCast SourceCast;
	public Flag Flags = 0;

	float createdAt = -1;
	float expiresAt = -1;

	public BaseBuff()
	{
		createdAt = Time.GetTicksMsec();
	}

	public static string MakeDescription(params string[] strings) => CastUtils.MakeDescription(strings);

	float defaultDuration;
	public float Duration
	{
		set
		{
			float time = Time.GetTicksMsec();
			expiresAt = time + value * Music.Singleton.SecondsPerBeat * 1000;
			defaultDuration = value;
		}
	}

	public void RefreshDuration()
	{
		Duration = defaultDuration;
	}

	public override void _Process(double delta)
	{
		if (expiresAt == -1)
			return;

		float time = Time.GetTicksMsec();
		if (time >= expiresAt)
		{
			ObjectBuffs.Remove(this);
		}
	}

	public virtual void ModifyUnit(BuffUnitStatsVisitor unit) { }
	public virtual void ModifyIncomingDamage(BuffIncomingDamageVisitor damage) { }
	public virtual void ModifyOutgoingDamage(BuffOutgoingDamageVisitor damage) { }
	public virtual void ModifyIncomingRestoration(BuffIncomingRestorationVisitor restoration) { }
	public virtual void ModifyOutgoingRestoration(BuffOutgoingRestorationVisitor restoration) { }
	public virtual void OnBeatTick(BeatTime time, int stacks) { }

	public enum Flag : int
	{
		SkillCreated = 1,
	}
}
