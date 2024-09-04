using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project;

public partial class DebugPanelUI : Control
{
	[Export] public RichTextLabel FramerateLabel;

	readonly List<double> frameTimes = new();

	public override void _PhysicsProcess(double delta)
	{
		frameTimes.Add(Engine.GetFramesPerSecond());
		if (frameTimes.Count > 20)
			frameTimes.RemoveAt(0);

		double average = frameTimes.Average();
		FramerateLabel.Text = $"[b]FPS:[/b] {average:0}";
	}
}
