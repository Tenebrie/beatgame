using Godot;

namespace Project;

public abstract partial class BaseBuff : Node
{
	public BaseUnit Parent;

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
			Parent.Buffs.Remove(this);
		}
	}

	public abstract void Visit(BuffVisitor visitor);
}
