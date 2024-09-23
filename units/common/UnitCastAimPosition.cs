using Godot;
using System;

namespace Project;

public partial class UnitCastAimPosition : Node3D
{
	public override void _Ready()
	{
		var parent = GetParent();
		if (parent != null && parent is BaseUnit unit)
		{
			unit.CastAimPosition = Position;
		}
		Visible = false;
	}
}
