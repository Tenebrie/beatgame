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
	[Export] public Label SeparatorLabel;
	[Export] public Label MaximumValueLabel;
	[Export] public Label OverkillLabel;
	[Export] public Timer PositiveTimer;
	[Export] public Timer NegativeTimer;

	private float CurrentValue = 100;
	private float MaximumValue = 100;
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
		PositiveTimer.WaitTime = 0.25f;
		NegativeTimer.WaitTime = 0.25f;
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
		PositiveGhostValue = value;
		// PositiveGhostValue = Math.Min(PositiveGhostValue, CurrentValue);
		NegativeGhostValue = Math.Max(NegativeGhostValue, CurrentValue);
		CurrentValueLabel.Text = Math.Round(value).ToString();

		Bar.Value = value;
	}

	public void SetOverflow(float value)
	{
		SetCurrent(value % MaximumValue);
		PositiveGhostValue = value;
		NegativeGhostValue = TrackedResource.Current;
		OverkillLabel.Text = $"+{Math.Round(value)}";
	}

	public void SetMaximum(float value)
	{
		MaximumValueLabel.Text = Math.Round(value).ToString();

		MaximumValue = value;
		Bar.MaxValue = value;
		PositiveGhost.MaxValue = value;
		NegativeGhost.MaxValue = value;
	}

	private BaseUnit TrackedUnit = null;
	private ObjectResource TrackedResource = null;

	public void TrackUnit(BaseUnit unit, ObjectResourceType resourceType)
	{
		var resource = (ObjectResource)unit.Composables.Find(script => script is ObjectResource resource && resource.Type == resourceType)
			?? throw new Exception("The requested resource does not exist on this unit.");

		TrackedUnit = unit;
		TrackedResource = resource;
		SetMaximum(resource.Maximum);
		SetCurrent(resource.Current);
		PositiveGhostValue = CurrentValue;
		NegativeGhostValue = CurrentValue;
		OverkillLabel.Visible = false;
		CurrentValueLabel.Visible = true;
		SeparatorLabel.Visible = true;
		MaximumValueLabel.Visible = true;
		unit.UnitKilled += OnTrackedUnitKilled;
		resource.ResourceChanged += OnResourceChanged;
		resource.ResourceRegenerated += OnResourceRegenerated;
		resource.MaxResourceChanged += OnMaxResourceChanged;
	}

	public void UntrackUnit()
	{
		if (TrackedUnit == null || TrackedResource == null)
			return;

		TrackedUnit.UnitKilled -= OnTrackedUnitKilled;
		TrackedResource.ResourceChanged -= OnResourceChanged;
		TrackedResource.ResourceRegenerated -= OnResourceRegenerated;
		TrackedResource.MaxResourceChanged -= OnMaxResourceChanged;

		TrackedUnit = null;
		TrackedResource = null;
		SetCurrent(0);
	}

	public override void _ExitTree()
	{
		UntrackUnit();
		SignalBus.Singleton.TrackStarted -= OnTrackStarted;
	}

	private void OnTrackedUnitKilled()
	{
		if (TrackedUnit.Alliance != UnitAlliance.Player)
		{
			OverkillLabel.Visible = true;
			CurrentValueLabel.Visible = false;
			SeparatorLabel.Visible = false;
			MaximumValueLabel.Visible = false;
		}
		SetOverflow(0);
	}

	private void OnResourceChanged(float value)
	{
		if (TrackedUnit.IsAlive)
			SetCurrent(value);
		else
			SetOverflow(TrackedResource.DamageOverflow);
	}

	private void OnResourceRegenerated(float value)
	{
		var diff = value - CurrentValue;
		if (diff > 0)
			PositiveGhostValue += diff;
		else
			NegativeGhostValue += diff;
		PositiveGhost.Value = PositiveGhostValue;
		NegativeGhost.Value = NegativeGhostValue;

		SetCurrent(value);
	}

	private void OnMaxResourceChanged(float value)
	{
		SetMaximum(value);
		PositiveGhostValue = CurrentValue;
		NegativeGhostValue = CurrentValue;
	}

	public override void _Process(double delta)
	{
		float fillSpeed = 150; // Units per second

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

		if (NegativeTimer.TimeLeft == 0)
		// if (true)
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
