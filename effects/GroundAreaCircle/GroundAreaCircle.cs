using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using Project;

namespace Project;
public partial class GroundAreaCircle : BaseTelegraph
{
	// private Decal outerCircle;
	private CircleDecal decal;
	private Area3D hitbox;
	private CollisionShape3D collisionShape;

	private float radius = .5f;

	public float Radius
	{
		get => radius;
		set
		{
			radius = value;
			UpdateRadius();
		}
	}

	public GroundAreaCircle()
	{
		createdAt = Time.GetTicksMsec();
	}

	public override void _EnterTree()
	{
		hitbox = GetNode<Area3D>("Hitbox");
		decal = GetNode<CircleDecal>("CircleDecal");
		collisionShape = GetNode<CollisionShape3D>("Hitbox/CollisionShape3D");
		// collisionShape.Shape = (Shape3D)collisionShape.Shape.Duplicate();

		hitbox.BodyEntered += OnBodyEntered;
		hitbox.BodyExited += OnBodyExited;

		base._EnterTree();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		decal.SetInstanceShaderParameter("PROGRESS", GrowPercentage);
	}

	private void UpdateRadius()
	{
		Scale = new Vector3(radius * 2, radius * 2, radius * 2);
		decal.SetInstanceShaderParameter("RADIUS", radius);
	}

	protected override void SetColor(Color color)
	{
		decal.SetInstanceShaderParameter("COLOR_R", color.R);
		decal.SetInstanceShaderParameter("COLOR_G", color.G);
		decal.SetInstanceShaderParameter("COLOR_B", color.B);
	}

	public override void CleanUp()
	{
		decal.CleanUp();
		decal.onFadedOut = () => base.CleanUp();
	}
}
