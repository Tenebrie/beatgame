using Godot;
using Project;
using System;
using System.Diagnostics;

public partial class TargetingCircle : Node3D
{
	[Export] public NodePath _decalPath = null;
	private Decal decal;

	public void SetRadius(float radius)
	{
		decal = GetNode<Decal>(_decalPath);
		Scale = new Vector3(radius, radius, radius);
	}
	public void SetAlliance(UnitAlliance alliance)
	{
		decal = GetNode<Decal>(_decalPath);
		var color = new Color(255, 255, 0);
		if (alliance == UnitAlliance.Player)
		{
			color = new Color(0, 0, 255);
		}
		else if (alliance == UnitAlliance.Enemy)
		{
			color = new Color(255, 0, 0);
		}
		decal.Modulate = color;
	}
}
