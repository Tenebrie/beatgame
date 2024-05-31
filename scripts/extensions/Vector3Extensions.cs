using System;
using Godot;

namespace Project;
public static class Vector3Extensions
{
	public static float FlatDistanceTo(this Vector3 a, Vector3 b)
	{
		return new Vector3(a.X, 0, a.Z).DistanceTo(new Vector3(b.X, 0, b.Z));
	}

	public static float VerticalDistanceTo(this Vector3 a, Vector3 b)
	{
		return Math.Abs(a.Y - b.Y);
	}

	public static Vector3 Flatten(this Vector3 a, float height)
	{
		return new Vector3(a.X, height, a.Z);
	}
}