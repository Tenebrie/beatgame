using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Project;

public abstract partial class BaseTelegraph : Node3D
{
	public bool Periodic;
	protected double createdAt;
	protected double finishesAt;
	protected bool endReached;
	protected bool cleaningUp;
	protected UnitAlliance alliance = UnitAlliance.Hostile;
	public Action<BaseUnit> OnTargetEntered = null;
	public Action OnFinishedCallback = null;
	public Func<BaseUnit, bool> TargetValidator = null;
	public Action<BaseUnit> OnFinishedPerTargetCallback = null;
	readonly List<BaseUnit> targets = new();

	protected float GrowPercentage;

	public UnitAlliance Alliance
	{
		get => alliance;
		set
		{
			alliance = value;
			UpdateColor();
		}
	}

	private float growTime = 1; // beats
	public float GrowTime
	{
		get => growTime;
		set
		{
			growTime = value;
			finishesAt = createdAt + Music.Singleton.SecondsPerBeat * value * 1000;
		}
	}

	public override void _Ready()
	{
		finishesAt = createdAt + Music.Singleton.SecondsPerBeat * GrowTime * 1000;
		UpdateColor();
	}

	public override void _Process(double delta)
	{
		var time = (double)Time.GetTicksMsec();
		GrowPercentage = (float)Math.Min(1, (time - createdAt) / (finishesAt - createdAt));

		if (GrowPercentage >= 1 && !endReached)
		{
			endReached = true;
			OnFinishedCallback?.Invoke();
			if (OnFinishedPerTargetCallback != null)
			{
				foreach (var target in GetTargets())
					OnFinishedPerTargetCallback(target);
			}
			if (!Periodic)
				CleanUp();
		}
	}

	protected void OnBodyEntered(Node3D body)
	{
		if (body is not BaseUnit unit)
			return;

		targets.Add(unit);
		if (TargetValidator == null || TargetValidator(unit))
			OnTargetEntered?.Invoke(unit);
	}

	protected void OnBodyExited(Node3D body)
	{
		if (body is not BaseUnit unit)
			return;

		targets.Remove(unit);
	}

	void UpdateColor()
	{
		SetColor(CastUtils.GetAllianceColor(alliance));
	}

	public List<BaseUnit> GetTargets()
	{
		return targets.Distinct().Where(target => target != null && IsInstanceValid(target) && TargetValidator == null || TargetValidator(target)).ToList();
	}

	public void SnapToGround()
	{
		GlobalPosition = CastUtils.SnapToGround(this, GlobalPosition);
	}

	protected abstract void SetColor(Color color);

	public virtual void CleanUp()
	{
		QueueFree();
	}
}