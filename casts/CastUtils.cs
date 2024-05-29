using System;
using System.Collections.Generic;
using Godot;
namespace Project;

public static class CastUtils
{

	public static GroundAreaCircle CreateGroundCircularArea(this Node node, Vector3 point)
	{
		var circle = Lib.Scene(Lib.Effect.GroundAreaCircle).Instantiate<GroundAreaCircle>();
		circle.Position = point;
		node.GetTree().CurrentScene.AddChild(circle);
		return circle;
	}

	public static GroundAreaRect CreateGroundRectangularArea(this Node node, Vector3 point)
	{
		var rect = Lib.Scene(Lib.Effect.GroundAreaRect).Instantiate<GroundAreaRect>();
		rect.Position = point;
		node.GetTree().CurrentScene.AddChild(rect);
		return rect;
	}

	public static float GetArenaSize(this Node _)
	{
		return 16;
	}

	/// <summary>
	/// Generates a position located at the edge of the arena with a given offset.
	/// <br />
	/// Use X coordinate for offset along the edge, and Z coordinate for distance from the edge.
	/// <br /><br />
	/// Units are relative, and should be in range (0; 1).
	/// <br /><br />
	/// With an offset of (0; 0; 0), the returned position is the middle of the corresponding edge.
	/// </summary>
	public static Vector3 GetArenaEdgePosition(this Node _, Vector3 offset, ArenaFacing facing)
	{
		var arenaSize = 16;
		offset *= arenaSize;
		if (facing == ArenaFacing.East)
			return new Vector3(-offset.Z + arenaSize, offset.Y, offset.X);
		if (facing == ArenaFacing.North)
			return new Vector3(offset.X, offset.Y, offset.Z - arenaSize);
		if (facing == ArenaFacing.West)
			return new Vector3(offset.Z - arenaSize, offset.Y, -offset.X);
		if (facing == ArenaFacing.South)
			return new Vector3(-offset.X, offset.Y, -offset.Z + arenaSize);
		return offset;
	}

	/// <summary>
	/// Returns an angle in radians corresponding to facing inside the arena.
	/// <br />
	/// Assumes that the target is facing towards negative Z axis (Y rotation is set to 0)
	/// <br />
	/// Usage: <code>Node3D.Rotate(Vector3.Up, this.GetArenaFacingAngle(...))</code>
	/// </summary>
	public static float GetArenaFacingAngle(this Node _, ArenaFacing facing)
	{
		if (facing == ArenaFacing.East)
			return (float)Math.PI / 2;
		if (facing == ArenaFacing.North)
			return (float)Math.PI;
		if (facing == ArenaFacing.West)
			return (float)-Math.PI / 2;
		if (facing == ArenaFacing.South)
			return 0;
		return 0;
	}

	public static List<ArenaFacing> AllArenaFacings()
	{
		return new() { ArenaFacing.East, ArenaFacing.North, ArenaFacing.West, ArenaFacing.South };
	}

	public static Vector3 SnapToGround(this Node3D node, Vector3 fromPos)
	{
		var spaceState = node.GetWorld3D().DirectSpaceState;
		var query = PhysicsRayQueryParameters3D.Create(fromPos + Vector3.Up * 5, fromPos + Vector3.Down * 10, 1 << 4);
		var result = spaceState.IntersectRay(query);
		if (result.Count == 0)
			return Vector3.Zero;

		return (Vector3)result["position"];
	}
}

public enum ArenaFacing : int
{
	East = 0,
	North = 1,
	West = 2,
	South = 3,
}