using Godot;
using System;
using System.Linq;

namespace Project;

public partial class AerialFaithPlate : Node3D
{
	CircularTelegraph circle;
	AerialFaithPlateTarget targetMarker;

	public override void _Ready()
	{
		circle = GetNode<CircularTelegraph>("GroundAreaCircle");
		circle.Settings.Alliance = UnitAlliance.Neutral;
		circle.Settings.Periodic = true;
		circle.Settings.GrowTime = 0.01f;
		circle.Settings.TargetValidator = (unit) => unit.Alliance == UnitAlliance.Player;
		circle.Settings.OnTargetEntered = OnTargetEntered;
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
