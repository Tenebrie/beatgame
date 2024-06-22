using System.Collections.Generic;
using Godot;

namespace Project;

public class CastTargetData
{
	public BaseUnit AlliedUnit;
	public BaseUnit HostileUnit;
	public Vector3 Point;
	public List<Vector3> MultitargetPoints = new();

	public static CastTargetData Empty()
	{
		return new CastTargetData();
	}
}
