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
	private CollisionShape3D hitboxCollision;

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

	public override void _Ready()
	{
		base._Ready();
		hitbox = GetNode<Area3D>("Hitbox");
		hitboxCollision = GetNode<CollisionShape3D>("Hitbox/CollisionShape3D");
		decal = GetNode<CircleDecal>("CircleDecal");

		hitbox.BodyEntered += OnBodyEntered;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		decal.SetInstanceShaderParameter("PROGRESS", GrowPercentage);
	}

	private void OnBodyEntered(Node3D body)
	{
		if (body is not BaseUnit unit)
			return;

		if (unit.Alliance.HostileTo(Alliance))
			OnHostileImpactCallback?.Invoke(unit);
	}

	public override List<BaseUnit> GetUnitsInside()
	{
		return BaseUnit.AllUnits.Where(unit =>
		{
			if (unit.Position.VerticalDistanceTo(Position) > 1)
				return false;

			return unit.Position.FlatDistanceTo(Position) < radius;
		}).ToList();
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
