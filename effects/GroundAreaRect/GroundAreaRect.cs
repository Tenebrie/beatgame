using System;
using System.Diagnostics;
using Godot;
using Project;

namespace Project;
public partial class GroundAreaRect : Node3D
{
	private Decal outerCircle;
	private Decal innerCircle;
	private double createdAt;
	private double finishesAt;
	private bool cleaningUp;

	private float width = 1;
	private float length = 1;
	private float rotation = 0;
	private float growTime = 1; // beats
	private Origin lengthOrigin = Origin.Center;
	private UnitAlliance alliance = UnitAlliance.Neutral;

	public float Width
	{
		get => width;
		set
		{
			width = value;
			UpdateSize();
		}
	}
	public float Length
	{
		get => length;
		set
		{
			length = value;
			UpdateSize();
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
	public Origin LengthOrigin
	{
		get => LengthOrigin;
		set
		{
			lengthOrigin = value;
			// TODO: Make that work properly
			if (value == Origin.Start)
			{
				outerCircle.Position = new Vector3(outerCircle.Position.X + Length / 2, outerCircle.Position.Y, outerCircle.Position.Z);
				innerCircle.Position = new Vector3(innerCircle.Position.X + Length / 2, innerCircle.Position.Y, innerCircle.Position.Z);
			}
		}
	}
	public Action OnFinishedCallback = null;

	public GroundAreaRect()
	{
		createdAt = Time.GetTicksMsec();
	}

	public override void _Ready()
	{
		outerCircle = GetNode<Decal>("OuterCircle");
		innerCircle = GetNode<Decal>("InnerCircle");
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
		innerCircle.Size = new Vector3(outerCircle.Size.X * percentage, 1, outerCircle.Size.Z * percentage);
		if (percentage >= 1)
		{
			OnFinishedCallback?.Invoke();
			cleaningUp = true;
		}
	}

	private void UpdateSize()
	{
		outerCircle.Size = new Vector3(length, 1, width);
		innerCircle.Size = new Vector3(length, 1, width);
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

	public enum Origin
	{
		Center,
		Start,
	}
}
