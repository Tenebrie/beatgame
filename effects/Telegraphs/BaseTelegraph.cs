using System;
using System.Collections.Generic;
using Godot;

namespace Project;

public abstract partial class BaseTelegraph : Node3D
{
	public bool Periodic;
	protected double createdAt;
	protected double finishesAt;
	protected bool endReached;
	protected bool cleaningUp;
	protected UnitAlliance alliance = UnitAlliance.Neutral;
	public Action<BaseUnit> OnHostileImpactCallback = null;
	public Action OnFinishedCallback = null;
	public Action<BaseUnit> OnFinishedPerTargetCallback = null;

	protected float GrowPercentage;

	public UnitAlliance Alliance
	{
		get => alliance;
		set
		{
			alliance = value;
			var color = new Color(255, 255, 0);

			if (alliance == UnitAlliance.Player)
				color = new Color(0, 0, 255);
			else if (alliance == UnitAlliance.Hostile)
				color = new Color(0.7f, 0.2f, 0);

			SetColor(color);
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
				foreach (var target in GetUnitsInside())
					OnFinishedPerTargetCallback(target);
			}
			if (!Periodic)
				CleanUp();
		}
	}

	public void SnapToGround()
	{
		GlobalPosition = CastUtils.SnapToGround(this, GlobalPosition);
	}

	protected abstract void SetColor(Color color);
	public abstract List<BaseUnit> GetUnitsInside();

	public virtual void CleanUp()
	{
		QueueFree();
	}
}