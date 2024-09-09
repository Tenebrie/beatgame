using Godot;
using Project;
using System;
using System.Diagnostics;

namespace Project;

public partial class TargetingCircle : Node3D
{
	private CircleDecal decal;

	public override void _Ready()
	{
		decal = GetNode<CircleDecal>("CircleDecal");
		decal.SetProgress(0);
		decal.SetInnerAlpha(0);
		decal.SetOuterWidth(1);
	}

	public override void _Process(double delta)
	{
		var verticalPos = this.SnapToGround(GlobalPosition).Y;
		GlobalPosition = new Vector3(GlobalPosition.X, verticalPos, GlobalPosition.Z);
	}

	public void SetRadius(float radius)
	{
		decal = GetNode<CircleDecal>("CircleDecal");
		decal.Radius = radius;
		decal.SetOuterWidth(decal.Radius * 7);
	}
	public void SetAlliance(UnitAlliance alliance)
	{
		decal = GetNode<CircleDecal>("CircleDecal");
		decal.SetColor(CastUtils.GetAltAllianceColor(alliance));
	}
}
