using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Project;
public partial class GroundAreaRect : BaseTelegraph
{
	private RectDecal decal;
	private Area3D hitbox;
	private CollisionShape3D collisionShape;

	private float width = 1;
	private float length = 1;
	private float rotation = 0;
	private Origin lengthOrigin = Origin.Center;

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

	public Origin LengthOrigin
	{
		get => LengthOrigin;
		set
		{
			lengthOrigin = value;
			// TODO: Make that work properly
			if (value == Origin.Start)
			{
				decal.Position = new Vector3(decal.Position.X, decal.Position.Y, decal.Position.Z - Length / 2);
			}
		}
	}

	public GroundAreaRect()
	{
		createdAt = Time.GetTicksMsec();
	}

	public override void _Ready()
	{
		hitbox = GetNode<Area3D>("Hitbox");
		decal = GetNode<RectDecal>("RectDecal");
		collisionShape = GetNode<CollisionShape3D>("Hitbox/CollisionShape3D");

		hitbox.BodyEntered += OnBodyEntered;
		hitbox.BodyExited += OnBodyExited;

		base._Ready();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		decal.SetInstanceShaderParameter("PROGRESS", GrowPercentage);
	}

	private void UpdateSize()
	{
		// Scale = new Vector3(width * 2, 1, length * 2);
		(decal.Mesh as PlaneMesh).Size = new Vector2(width * 2, length * 2);
		(collisionShape.Shape as BoxShape3D).Size = new Vector3(width, width + length, length);
		decal.SetInstanceShaderParameter("SIZE_X", width / 2);
		decal.SetInstanceShaderParameter("SIZE_Z", length / 2);
	}

	protected override void SetColor(Color color)
	{
		decal.SetInstanceShaderParameter("COLOR_R", color.R);
		decal.SetInstanceShaderParameter("COLOR_G", color.G);
		decal.SetInstanceShaderParameter("COLOR_B", color.B);
	}

	// public static bool IsPointInsideRectangle(Vector3 pt, Vector3 A, Vector3 B, Vector3 C, Vector3 D)
	// {
	// 	double x1 = A.X;
	// 	double x2 = B.X;
	// 	double x3 = C.X;
	// 	double x4 = D.X;

	// 	double y1 = A.Z;
	// 	double y2 = B.Z;
	// 	double y3 = C.Z;
	// 	double y4 = D.Z;

	// 	double a1 = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
	// 	double a2 = Math.Sqrt((x2 - x3) * (x2 - x3) + (y2 - y3) * (y2 - y3));
	// 	double a3 = Math.Sqrt((x3 - x4) * (x3 - x4) + (y3 - y4) * (y3 - y4));
	// 	double a4 = Math.Sqrt((x4 - x1) * (x4 - x1) + (y4 - y1) * (y4 - y1));

	// 	double b1 = Math.Sqrt((x1 - pt.X) * (x1 - pt.X) + (y1 - pt.Z) * (y1 - pt.Z));
	// 	double b2 = Math.Sqrt((x2 - pt.X) * (x2 - pt.X) + (y2 - pt.Z) * (y2 - pt.Z));
	// 	double b3 = Math.Sqrt((x3 - pt.X) * (x3 - pt.X) + (y3 - pt.Z) * (y3 - pt.Z));
	// 	double b4 = Math.Sqrt((x4 - pt.X) * (x4 - pt.X) + (y4 - pt.Z) * (y4 - pt.Z));

	// 	double u1 = (a1 + b1 + b2) / 2;
	// 	double u2 = (a2 + b2 + b3) / 2;
	// 	double u3 = (a3 + b3 + b4) / 2;
	// 	double u4 = (a4 + b4 + b1) / 2;

	// 	double A1 = Math.Sqrt(u1 * (u1 - a1) * (u1 - b1) * (u1 - b2));
	// 	double A2 = Math.Sqrt(u2 * (u2 - a2) * (u2 - b2) * (u2 - b3));
	// 	double A3 = Math.Sqrt(u3 * (u3 - a3) * (u3 - b3) * (u3 - b4));
	// 	double A4 = Math.Sqrt(u4 * (u4 - a4) * (u4 - b4) * (u4 - b1));

	// 	double difference = A1 + A2 + A3 + A4 - a1 * a2;
	// 	return difference < 1;
	// }

	public enum Origin
	{
		Center,
		Start,
	}
}
