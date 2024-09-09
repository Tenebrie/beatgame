using Godot;

namespace Project;

public static class Raycast
{
	public enum Layer : uint
	{
		Base = 1 << 0,
		Floors = 1 << 4,
		Walls = 1 << 5,
		Ceilings = 1 << 6,
	}

	public static Vector3 GetFirstHitPositionGlobal(Node3D baseNode, Vector3 fromPos, Vector3 toPos, Layer layer)
	{
		var spaceState = baseNode.GetWorld3D().DirectSpaceState;
		var query = PhysicsRayQueryParameters3D.Create(fromPos, toPos, (uint)layer);
		var result = spaceState.IntersectRay(query);
		if (result.Count == 0)
			return Vector3.Zero;

		return (Vector3)result["position"];
	}

	public static Vector3 GetFirstHitPositionRelative(Node3D baseNode, Vector3 fromPos, Vector3 toPos, Layer layer)
	{
		return GetFirstHitPositionGlobal(baseNode, baseNode.GlobalPosition + fromPos, baseNode.GlobalPosition + toPos, layer) - baseNode.GlobalPosition;
	}
}