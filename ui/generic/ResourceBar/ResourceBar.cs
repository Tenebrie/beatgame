using Godot;
using System;

public partial class ResourceBar : Control
{
	public ProgressBar Bar;
	public ProgressBar PositiveGhost;
	public ProgressBar NegativeGhost;
	public Label CurrentValueLabel;
	public Label MaximumValueLabel;

	private float CurrentValue = 50;
	private float PositiveGhostValue = 25;
	private float NegativeGhostValue = 75;
	private float MaximumValue = 100;
	public override void _Ready()
	{
		Bar = GetNode<ProgressBar>("Bar");
		PositiveGhost = GetNode<ProgressBar>("PositiveGhost");
		NegativeGhost = GetNode<ProgressBar>("NegativeGhost");
		CurrentValueLabel = GetNode<Label>("CurrentLabel");
		MaximumValueLabel = GetNode<Label>("MaximumLabel");
	}

	public void SetCurrent(float value)
	{
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

	public override void _Process(double delta)
	{
		float fillSpeed = 15; // Units per second

		PositiveGhostValue += fillSpeed * (float)delta;
		if (PositiveGhostValue >= CurrentValue)
		{
			PositiveGhostValue = CurrentValue;
		}

		NegativeGhostValue -= fillSpeed * (float)delta;
		if (NegativeGhostValue <= CurrentValue)
		{
			NegativeGhostValue = CurrentValue;
		}

		PositiveGhost.Value = PositiveGhostValue;
		NegativeGhost.Value = NegativeGhostValue;
	}
}
