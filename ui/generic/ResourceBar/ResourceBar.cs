using Godot;
using System;
using System.Diagnostics;

namespace Project;
public partial class ResourceBar : Control
{
	ProgressBar Bar;
	ProgressBar PositiveGhost;
	ProgressBar NegativeGhost;
	Label PositiveComboLabel;
	Label NegativeComboLabel;
	[Export] public ThemedProgressBar ThemedProgressBar;
	[Export] public Label CurrentValueLabel;
	[Export] public Label MaximumValueLabel;
	[Export] public Timer PositiveTimer;
	[Export] public Timer NegativeTimer;

	private float CurrentValue = 100;
	private float PositiveGhostValue = 100;
	private float NegativeGhostValue = 100;

	public override void _EnterTree()
	{
		Bar = ThemedProgressBar.Bar;
		PositiveGhost = ThemedProgressBar.PositiveGhost;
		NegativeGhost = ThemedProgressBar.NegativeGhost;
		PositiveComboLabel = ThemedProgressBar.PositiveComboLabel;
		NegativeComboLabel = ThemedProgressBar.NegativeComboLabel;
	}

	public override void _Ready()
	{
		PositiveTimer.OneShot = true;
		NegativeTimer.OneShot = true;
		PositiveTimer.WaitTime = 1;
		NegativeTimer.WaitTime = 1;
		SignalBus.Singleton.TrackStarted += OnTrackStarted;
	}

	private void OnTrackStarted(MusicTrack track)
	{
		PositiveTimer.WaitTime = 1.00f;
		NegativeTimer.WaitTime = 1.00f;
	}

	public void SetCurrent(float value)
	{
		if (value > CurrentValue)
		{
			PositiveTimer.Start();
		}
		else if (value < CurrentValue)
		{
			NegativeTimer.Start();
		}

		CurrentValue = value;
		PositiveGhostValue = Math.Min(PositiveGhostValue, CurrentValue);
		NegativeGhostValue = Math.Max(NegativeGhostValue, CurrentValue);
		CurrentValueLabel.Text = Math.Round(value).ToString();

		Bar.Value = value;
	}

	public void SetMaximum(float value)
	{
		MaximumValueLabel.Text = Math.Round(value).ToString();

		Bar.MaxValue = value;
		PositiveGhost.MaxValue = value;
		NegativeGhost.MaxValue = value;
	}

	private BaseUnit TrackedUnit = null;
	private ObjectResourceType? TrackedResource = null;

	public void TrackUnit(BaseUnit unit, ObjectResourceType resourceType)
	{
		var resource = (ObjectResource)unit.Composables.Find(script => script is ObjectResource resource && resource.Type == resourceType)
			?? throw new Exception("The requested resource does not exist on this unit.");

		TrackedUnit = unit;
		TrackedResource = resourceType;
		SetMaximum(resource.Maximum);
		SetCurrent(resource.Current);
		PositiveGhostValue = CurrentValue;
		NegativeGhostValue = CurrentValue;
		SignalBus.Singleton.ResourceChanged += OnResourceChanged;
		SignalBus.Singleton.ResourceRegenerated += OnResourceRegenerated;
		SignalBus.Singleton.MaxResourceChanged += OnMaxResourceChanged;
	}

	public void UntrackUnit()
	{
		if (TrackedUnit == null || TrackedResource == null)
			return;

		TrackedUnit = null;
		TrackedResource = null;
		SignalBus.Singleton.ResourceChanged -= OnResourceChanged;
		SignalBus.Singleton.ResourceRegenerated -= OnResourceRegenerated;
		SignalBus.Singleton.MaxResourceChanged -= OnMaxResourceChanged;
		SetCurrent(0);
	}

	public override void _ExitTree()
	{
		UntrackUnit();
		SignalBus.Singleton.TrackStarted -= OnTrackStarted;
	}

	private void OnResourceChanged(BaseUnit unit, ObjectResourceType type, float value)
	{
		if (unit != TrackedUnit || type != TrackedResource)
			return;

		SetCurrent(value);
	}

	private void OnResourceRegenerated(BaseUnit unit, ObjectResourceType type, float value)
	{
		if (unit != TrackedUnit || type != TrackedResource)
			return;

		var diff = value - CurrentValue;
		if (diff > 0)
			PositiveGhostValue += diff;
		else
			NegativeGhostValue += diff;
		PositiveGhost.Value = PositiveGhostValue;
		NegativeGhost.Value = NegativeGhostValue;

		SetCurrent(value);
	}

	private void OnMaxResourceChanged(BaseUnit unit, ObjectResourceType type, float value)
	{
		if (unit != TrackedUnit || type != TrackedResource)
			return;

		SetMaximum(value);
		PositiveGhostValue = CurrentValue;
		NegativeGhostValue = CurrentValue;
	}

	public override void _Process(double delta)
	{
		float fillSpeed = 50; // Units per second

		// if (PositiveTimer.TimeLeft == 0)
		if (true)
		{
			PositiveGhostValue += fillSpeed * (float)delta;
		}
		if (PositiveGhostValue >= CurrentValue)
		{
			PositiveGhostValue = CurrentValue;
			PositiveTimer.Stop();
		}

		// if (NegativeTimer.TimeLeft == 0)
		if (true)
		{
			NegativeGhostValue -= fillSpeed * (float)delta;
		}
		if (NegativeGhostValue <= CurrentValue)
		{
			NegativeGhostValue = CurrentValue;
			NegativeTimer.Stop();
		}

		Bar.Value = CurrentValue;
		PositiveGhost.Value = PositiveGhostValue;
		NegativeGhost.Value = NegativeGhostValue;

		var positiveValue = Math.Round(CurrentValue - PositiveGhostValue);
		PositiveComboLabel.Text = positiveValue > 0 ? Math.Round(positiveValue).ToString() : "";
		var negativeValue = Math.Round(NegativeGhostValue - CurrentValue);
		NegativeComboLabel.Text = negativeValue > 0 ? Math.Round(negativeValue).ToString() : "";
	}
}
