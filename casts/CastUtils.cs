using System;
using Godot;
namespace Project;

public static class CastUtils
{

	public static GroundAreaCircle CreateGroundCircularArea(this Node node, Vector3 point)
	{
		var circle = Lib.Scene(Lib.Effect.GroundAreaCircle).Instantiate<GroundAreaCircle>();
		circle.Position = point;
		node.GetTree().Root.AddChild(circle);
		return circle;
	}

	public static GroundAreaRect CreateGroundRectangularArea(this Node node, Vector3 point)
	{
		var rect = Lib.Scene(Lib.Effect.GroundAreaRect).Instantiate<GroundAreaRect>();
		rect.Position = point;
		node.GetTree().Root.AddChild(rect);
		return rect;
	}

	/// <summary>
	/// Generates a position located at the edge of the arena with a given offset.
	/// <br />
	/// Use X coordinate for offset along the edge, and Z coordinate for distance from the edge.
	/// <br /><br />
	/// With an offset of (0; 0; 0), the returned position is the middle of the corresponding edge.
	/// </summary>
	public static Vector3 RotatePositionToArenaEdge(this Node _, Vector3 offset, ArenaFacing facing)
	{
		var arenaSize = 16;
		if (facing == ArenaFacing.East)
			return new Vector3(offset.Z + arenaSize, offset.Y, offset.X);
		if (facing == ArenaFacing.North)
			return new Vector3(offset.X, offset.Y, -offset.Z - arenaSize);
		if (facing == ArenaFacing.West)
			return new Vector3(-offset.Z - arenaSize, offset.Y, offset.X);
		if (facing == ArenaFacing.South)
			return new Vector3(offset.X, offset.Y, offset.Z + arenaSize);
		return offset;
	}

	/// <summary>
	/// Returns an angle in radians corresponding to facing inside the arena.
	/// <br />
	/// Assumes that the target is facing towards positive X axis (Y rotation is set to 0)
	/// <br />
	/// Usage: <code>Node3D.Rotate(Vector3.Up, this.GetArenaFacingAngle(...))</code>
	/// </summary>
	public static float GetArenaFacingAngle(this Node _, ArenaFacing facing)
	{
		if (facing == ArenaFacing.East)
			return (float)Math.PI;
		if (facing == ArenaFacing.North)
			return (float)-Math.PI / 2;
		if (facing == ArenaFacing.West)
			return 0;
		if (facing == ArenaFacing.South)
			return (float)Math.PI / 2;
		return 0;
	}
}

public enum ArenaFacing : int
{
	East = 0,
	North = 1,
	West = 2,
	South = 3,
}