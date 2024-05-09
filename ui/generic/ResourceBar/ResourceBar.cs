using Godot;
using System;
using System.Diagnostics;

namespace Project;
public partial class ResourceBar : Control
{
	public ProgressBar Bar;
	public ProgressBar PositiveGhost;
	public ProgressBar NegativeGhost;
	public Label CurrentValueLabel;
	public Label MaximumValueLabel;
	public Timer PositiveTimer;
	public Timer NegativeTimer;
	public Label PositiveComboLabel;
	public Label NegativeComboLabel;

	private float CurrentValue = 100;
	private float PositiveGhostValue = 100;
	private float NegativeGhostValue = 100;
	private float MaximumValue = 100;
	public override void _Ready()
	{
		Bar = GetNode<ProgressBar>("Bar");
		PositiveGhost = GetNode<ProgressBar>("PositiveGhost");
		NegativeGhost = GetNode<ProgressBar>("NegativeGhost");
		CurrentValueLabel = GetNode<Label>("CurrentLabel");
		MaximumValueLabel = GetNode<Label>("MaximumLabel");
		PositiveTimer = GetNode<Timer>("PositiveTimer");
		NegativeTimer = GetNode<Timer>("NegativeTimer");
		PositiveTimer.OneShot = true;
		NegativeTimer.OneShot = true;
		PositiveComboLabel = GetNode<Label>("PositiveComboLabel");
		NegativeComboLabel = GetNode<Label>("NegativeComboLabel");
	}

	public void SetCurrent(float value)
	{
		if (value > CurrentValue)
		{
			PositiveTimer.Start(0.8);
		}
		else if (value < CurrentValue)
		{
			NegativeTimer.Start(0.8);
		}

		CurrentValue = value;
		PositiveGhostValue = Math.Min(PositiveGhostValue, CurrentValue);
		NegativeGhostValue = Math.Max(NegativeGhostValue, CurrentValue);
		CurrentValueLabel.Text = value.ToString();

		Bar.Value = value;
	}

	public void SetMaximum(float value)
	{
		MaximumValueLabel.Text = value.ToString();

		Bar.MaxValue = value;
		PositiveGhost.MaxValue = value;
		NegativeGhost.MaxValue = value;
	}

	private BaseUnit TrackedUnit = null;
	private ObjectResourceType? TrackedResource = null;

	public void TrackUnit(BaseUnit unit, ObjectResourceType resourceType)
	{
		TrackedUnit = unit;
		TrackedResource = resourceType;
		SetCurrent(unit.Health.Current);
		SetMaximum(unit.Health.Maximum);
		PositiveGhostValue = CurrentValue;
		NegativeGhostValue = CurrentValue;
		SignalBus.GetInstance(this).ResourceChanged += OnResourceChanged;
		SignalBus.GetInstance(this).MaxResourceChanged += OnMaxResourceChanged;
	}

	public void UntrackUnit()
	{
		if (TrackedUnit == null || TrackedResource == null)
			return;

		TrackedUnit = null;
		TrackedResource = null;
		SignalBus.GetInstance(this).ResourceChanged -= OnResourceChanged;
		SignalBus.GetInstance(this).MaxResourceChanged -= OnMaxResourceChanged;
		SetCurrent(0);
	}

	public override void _ExitTree()
	{
		UntrackUnit();
	}

	private void OnResourceChanged(BaseUnit unit, ObjectResourceType type, float value)
	{
		if (unit != TrackedUnit || type != TrackedResource)
			return;

		SetCurrent(value);
	}

	private void OnMaxResourceChanged(BaseUnit unit, ObjectResourceType type, float value)
	{
		if (unit != TrackedUnit || type != TrackedResource)
			return;

		SetMaximum(value);
	}

	public override void _Process(double delta)
	{
		float fillSpeed = 50; // Units per second

		if (PositiveTimer.TimeLeft == 0)
		{
			PositiveGhostValue += fillSpeed * (float)delta;
		}
		if (PositiveGhostValue >= CurrentValue)
		{
			PositiveGhostValue = CurrentValue;
			PositiveTimer.Stop();
		}

		if (NegativeTimer.TimeLeft == 0)
		{
			NegativeGhostValue -= fillSpeed * (float)delta;
		}
		if (NegativeGhostValue <= CurrentValue)
		{
			NegativeGhostValue = CurrentValue;
			NegativeTimer.Stop();
		}

		PositiveGhost.Value = PositiveGhostValue;
		NegativeGhost.Value = NegativeGhostValue;

		var positiveValue = Math.Round(CurrentValue - PositiveGhostValue);
		PositiveComboLabel.Text = positiveValue > 0 ? positiveValue.ToString() : "";
		var negativeValue = Math.Round(NegativeGhostValue - CurrentValue);
		NegativeComboLabel.Text = negativeValue > 0 ? negativeValue.ToString() : "";
	}
}
