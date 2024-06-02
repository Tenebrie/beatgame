using Godot;
using System;
using System.Linq;

namespace Project;

public partial class AerialFaithPlate : Node3D
{
	GroundAreaCircle circle;
	AerialFaithPlateTarget targetMarker;

	public override void _Ready()
	{
		circle = GetNode<GroundAreaCircle>("GroundAreaCircle");
		circle.Alliance = UnitAlliance.Neutral;
		circle.Periodic = true;
		circle.GrowTime = 0.01f;
		circle.TargetValidator = (unit) => unit.Alliance == UnitAlliance.Player;
		circle.OnTargetEntered = OnTargetEntered;
		targetMarker = GetChildren().Where(child => child is AerialFaithPlateTarget).Cast<AerialFaithPlateTarget>().First();
		if (targetMarker == null)
			GD.PushError("No AerialFaithPlateTarget child found for this AerialFaithPlate");
	}

	void OnTargetEntered(BaseUnit unit)
	{
		var target = targetMarker.GlobalPosition - unit.GlobalPosition;
		unit.ForcefulMovement.Push(target.Length(), target, 2);
	}
}
