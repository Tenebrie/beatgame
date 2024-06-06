using Godot;
using System;
using System.Diagnostics;

namespace Project;

[Tool]
public partial class CentralCircle : Control
{
	private Color BaseColor = new(255, 255, 255);
	private Color BeatColor = new(0, 255, 255);
	private Color HalfBeatColor = new(0, 255, 255);
	public override void _Draw()
	{
		// Half beat backgrounds
		DrawArc(new Vector2(-50, 0), 20, -(float)Math.PI / 2 + 0.3f, (float)Math.PI / 2 - 0.3f, 50, BaseColor, 40, true);
		DrawArc(new Vector2(50, 0), 20, (float)Math.PI / 2 + 0.3f, (float)Math.PI + (float)Math.PI / 2 - 0.3f, 50, BaseColor, 40, true);
		// Half beat foreground
		DrawArc(new Vector2(-50, 0), 20, -(float)Math.PI / 2 + 0.3f, (float)Math.PI / 2 - 0.3f, 50, HalfBeatColor, 40, true);
		DrawArc(new Vector2(50, 0), 20, (float)Math.PI / 2 + 0.3f, (float)Math.PI + (float)Math.PI / 2 - 0.3f, 50, HalfBeatColor, 40, true);

		// Full beat background
		DrawArc(new Vector2(0, 0), 50, 0, (float)Math.PI * 2, 50, BaseColor, 10, true);
		// Full beat foreground
		DrawArc(new Vector2(0, 0), 50, 0, (float)Math.PI * 2, 50, BeatColor, 10, true);
	}

	public override void _Ready()
	{
		if (Engine.IsEditorHint())
			return;

		Music.Singleton.BeatTick += OnBeatTick;
	}

	public void OnBeatTick(BeatTime time)
	{
		if (time == BeatTime.Half || time == BeatTime.Whole)
			BeatColor.A = 1;
		else if (time == BeatTime.Quarter)
			HalfBeatColor.A = 1;
	}

	public override void _Process(double delta)
	{
		if (Engine.IsEditorHint())
			return;

		// No redraw necessary, skip this frame
		if (BeatColor.A == 0 && HalfBeatColor.A == 0)
			return;

		BeatColor.A = Math.Max(0, BeatColor.A - 3 * (float)delta);
		HalfBeatColor.A = Math.Max(0, HalfBeatColor.A - 3 * (float)delta);
		QueueRedraw();
	}
}
