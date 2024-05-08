using Godot;
using System;
using System.Diagnostics;

namespace Project;
public partial class UnitCard : Control
{
	public Label NameLabel;
	public ResourceBar HealthBar;
	public BaseUnit TrackedUnit;

	public override void _Ready()
	{
		NameLabel = GetNode<Label>("NameLabel");
		HealthBar = GetNode<ResourceBar>("HealthBar");

		if (TrackedUnit != null) {
			NameLabel.Text = TrackedUnit.FriendlyName;
			HealthBar.TrackUnit(TrackedUnit, ObjectResourceType.Health);
		}
	}

	public void TrackUnit(BaseUnit unit)
	{
		TrackedUnit = unit;

		if (NameLabel != null) {
			NameLabel.Text = TrackedUnit.FriendlyName;
			HealthBar.TrackUnit(TrackedUnit, ObjectResourceType.Health);
		}
	}

	public void UntrackUnit()
	{
		HealthBar.UntrackUnit();
	}
}
