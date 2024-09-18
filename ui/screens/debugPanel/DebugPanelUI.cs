using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project;

public partial class DebugPanelUI : Control
{
	[Export] public RichTextLabel FramerateLabel;

	int lastSeenFrameCount = 0;
	readonly List<double> latestFrameTimes = new();

	public override void _Process(double delta)
	{
		var frameDelta = Engine.GetFramesDrawn() - lastSeenFrameCount;
		lastSeenFrameCount = Engine.GetFramesDrawn();
		var currentFrameRate = frameDelta / delta;

		latestFrameTimes.Add(currentFrameRate);
		if (latestFrameTimes.Count > 100)
			latestFrameTimes.RemoveAt(0);

		double average = latestFrameTimes.Average();
		double min = latestFrameTimes.Min();
		FramerateLabel.Text = $"[b]Avg. FPS:[/b] {average:0}, [b]Min FPS:[/b] {min:0}";
	}

	public override void _UnhandledKeyInput(InputEvent @event)
	{
		if (@event.IsActionPressed("ToggleUI".ToStringName()))
		{
			Visible = !Visible;
		}
	}
}
