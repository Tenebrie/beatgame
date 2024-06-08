using System.Collections.Generic;
using Godot;

namespace Project;

public abstract partial class BaseBuff : Node
{
	public BaseUnit Parent;
	public Flag Flags = 0;

	float createdAt = -1;
	float expiresAt = -1;

	public BaseBuff()
	{
		createdAt = Time.GetTicksMsec();
	}

	public float Duration
	{
		set
		{
			float time = Time.GetTicksMsec();
			expiresAt = time + value * Music.Singleton.SecondsPerBeat * 1000;
		}
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

	public enum Flag : int
	{
		SkillCreated = 1,
	}
}
