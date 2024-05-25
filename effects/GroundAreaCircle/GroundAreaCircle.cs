using System;
using System.Diagnostics;
using Godot;
using Project;

namespace Project;
public partial class GroundAreaCircle : Node3D
{
	private Decal outerCircle;
	private Decal innerCircle;
	private double createdAt;
	private double finishesAt;
	private bool cleaningUp;

	private float radius = .5f;
	private float growTime = 1; // beats
	private UnitAlliance alliance = UnitAlliance.Neutral;

	public float Radius
	{
		get => radius;
		set
		{
			radius = value;
			UpdateRadius();
		}
	}
	public UnitAlliance Alliance
	{
		get => alliance;
		set
		{
			alliance = value;
			UpdateAlliance();
		}
	}
	public float GrowTime
	{
		get => growTime;
		set
		{
			growTime = value;
			finishesAt = createdAt + Music.Singleton.SecondsPerBeat * value * 1000;
		}
	}
	public Action OnFinishedCallback = null;

	public GroundAreaCircle()
	{
		createdAt = Time.GetTicksMsec();
	}

	public override void _Ready()
	{
		outerCircle = GetNode<Decal>("OuterCircle");
		innerCircle = GetNode<Decal>("InnerCircle");
		innerCircle.Scale = new Vector3(0, 0, 0);
		finishesAt = createdAt + Music.Singleton.SecondsPerBeat * GrowTime * 1000;
	}

	public override void _Process(double delta)
	{
		if (cleaningUp)
		{
			QueueFree();

			return;
		}

		var time = (double)Time.GetTicksMsec();
		var percentage = (float)Math.Min(1, (time - createdAt) / (finishesAt - createdAt));
		innerCircle.Scale = new Vector3(percentage, 1, percentage);
		if (percentage >= 1)
		{
			OnFinishedCallback?.Invoke();
			cleaningUp = true;
		}
	}

	private void UpdateRadius()
	{
		Scale = new Vector3(radius * 2, radius * 2, radius * 2);
	}

	private void UpdateAlliance()
	{
		var color = new Color(255, 255, 0);

		if (alliance == UnitAlliance.Player)
			color = new Color(0, 0, 255);
		else if (alliance == UnitAlliance.Hostile)
			color = new Color(255, 0, 0, 0.5f);
		outerCircle.Modulate = color;
		outerCircle.AlbedoMix = 1;
		innerCircle.Modulate = color;
		innerCircle.AlbedoMix = 0;
	}
}
