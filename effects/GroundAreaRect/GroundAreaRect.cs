using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
				outerCircle.Position = new Vector3(outerCircle.Position.X, outerCircle.Position.Y, outerCircle.Position.Z - Length / 2);
				innerCircle.Position = new Vector3(innerCircle.Position.X, innerCircle.Position.Y, innerCircle.Position.Z - Length / 2);
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
		outerCircle.Size = new Vector3(width, 1, length);
		innerCircle.Size = new Vector3(width, 1, length);
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

	public List<BaseUnit> GetUnitsInside()
	{
		var forward = -GlobalTransform.Basis.Z;
		var right = GlobalTransform.Basis.X;
		return BaseUnit.AllUnits.Where(unit =>
		{
			if (unit.Position.VerticalDistanceTo(Position) > 1)
				return false;

			var pos = innerCircle.GlobalPosition;
			var A = pos + forward * Length / 2 + right * Width / 2;
			var B = pos + forward * Length / 2 - right * Width / 2;
			var C = pos - forward * Length / 2 + right * Width / 2;
			var D = pos - forward * Length / 2 - right * Width / 2;

			return IsPointInsideRectangle(unit.Position, A, B, C, D);
		}).ToList();
	}

	public static bool IsPointInsideRectangle(Vector3 pt, Vector3 A, Vector3 B, Vector3 C, Vector3 D)
	{
		double x1 = A.X;
		double x2 = B.X;
		double x3 = C.X;
		double x4 = D.X;

		double y1 = A.Z;
		double y2 = B.Z;
		double y3 = C.Z;
		double y4 = D.Z;

		double a1 = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
		double a2 = Math.Sqrt((x2 - x3) * (x2 - x3) + (y2 - y3) * (y2 - y3));
		double a3 = Math.Sqrt((x3 - x4) * (x3 - x4) + (y3 - y4) * (y3 - y4));
		double a4 = Math.Sqrt((x4 - x1) * (x4 - x1) + (y4 - y1) * (y4 - y1));

		double b1 = Math.Sqrt((x1 - pt.X) * (x1 - pt.X) + (y1 - pt.Z) * (y1 - pt.Z));
		double b2 = Math.Sqrt((x2 - pt.X) * (x2 - pt.X) + (y2 - pt.Z) * (y2 - pt.Z));
		double b3 = Math.Sqrt((x3 - pt.X) * (x3 - pt.X) + (y3 - pt.Z) * (y3 - pt.Z));
		double b4 = Math.Sqrt((x4 - pt.X) * (x4 - pt.X) + (y4 - pt.Z) * (y4 - pt.Z));

		double u1 = (a1 + b1 + b2) / 2;
		double u2 = (a2 + b2 + b3) / 2;
		double u3 = (a3 + b3 + b4) / 2;
		double u4 = (a4 + b4 + b1) / 2;

		double A1 = Math.Sqrt(u1 * (u1 - a1) * (u1 - b1) * (u1 - b2));
		double A2 = Math.Sqrt(u2 * (u2 - a2) * (u2 - b2) * (u2 - b3));
		double A3 = Math.Sqrt(u3 * (u3 - a3) * (u3 - b3) * (u3 - b4));
		double A4 = Math.Sqrt(u4 * (u4 - a4) * (u4 - b4) * (u4 - b1));

		double difference = A1 + A2 + A3 + A4 - a1 * a2;
		return difference < 1;
	}

	public enum Origin
	{
		Center,
		Start,
	}
}
