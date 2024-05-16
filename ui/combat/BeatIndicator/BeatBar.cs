using Godot;
using System;

namespace Project;
public partial class BeatBar : Control
{
	enum State
	{
		Spawning,
		Normal,
		CleaningUp,
		Hidden,
	}
	private State VisualState;
	public bool Mirrored;
	public bool CleaningUp;
	public Color DrawColor = new(255, 255, 255, 0);
	public BeatBar(bool mirrored)
	{
		Mirrored = mirrored;
	}
	public override void _Draw()
	{
		if (VisualState == State.CleaningUp)
		{
			DrawArc(new Vector2(0, 0), 50, 0, (float)Math.PI * 2, 50, DrawColor, 10, true);
		}
		else if (Mirrored)
		{
			// DrawArc(new Vector2(0, 0), 50, (float)Math.PI * 1.5f, (float)Math.PI * 2.5f, 50, DrawColor, 10, true);
			DrawArc(new Vector2(0, 0), 50, (float)Math.PI * 1.75f, (float)Math.PI * 2.25f, 50, DrawColor, 10, true);
		}
		else
		{
			// DrawArc(new Vector2(0, 0), 50, (float)Math.PI / 2, (float)Math.PI * 1.5f, 50, DrawColor, 10, true);
			DrawArc(new Vector2(0, 0), 50, (float)Math.PI * 0.75f, (float)Math.PI * 1.25f, 50, DrawColor, 10, true);
		}
	}

	public override void _Process(double delta)
	{
		if (VisualState == State.Spawning)
		{
			DrawColor = new Color(DrawColor.R, DrawColor.G, DrawColor.B, DrawColor.A + 1 * (float)delta);
			QueueRedraw();

			if (DrawColor.A >= 0.5f)
			{
				DrawColor = new Color(DrawColor.R, DrawColor.G, DrawColor.B, 0.5f);
				VisualState = State.Normal;
			}
		}

		if (VisualState == State.CleaningUp)
		{
			DrawColor = new Color(DrawColor.R, DrawColor.G, DrawColor.B, Math.Max(0, DrawColor.A - 3 * (float)delta));
			QueueRedraw();

			if (DrawColor.A <= 0.00f)
			{
				VisualState = State.Hidden;
				QueueFree();
			}
		}
	}

	public void Cleanup()
	{
		DrawColor = new Color(0, 255, 255);
		VisualState = State.CleaningUp;
		// await ToSignal(GetTree().CreateTimer(2), "timeout");
		// QueueFree();
	}
}
