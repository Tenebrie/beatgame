using Godot;
using System;

namespace Project;
public partial class BeatBar : Control
{
	enum State
	{
		Spawning,
		Normal,
		Hidden,
	}
	private State VisualState;
	public bool Mirrored;
	public bool HalfBeat;
	public Color DrawColor = new(255, 255, 255, 0);

	public long StartsAt;
	public long EndsAt;

	public BeatBar(bool mirrored, bool halfBeat)
	{
		Mirrored = mirrored;
		HalfBeat = halfBeat;
	}
	public override void _Draw()
	{
		if (HalfBeat)
		{
			var position = Mirrored ? 45 : -45;
			DrawCircle(new Vector2(position, 0), 10, DrawColor);
		}
		else if (Mirrored)
		{
			DrawArc(new Vector2(0, 0), 50, (float)Math.PI * 1.75f, (float)Math.PI * 2.25f, 50, DrawColor, 10, true);
		}
		else
		{
			DrawArc(new Vector2(0, 0), 50, (float)Math.PI * 0.75f, (float)Math.PI * 1.25f, 50, DrawColor, 10, true);
		}
	}

	public override void _Process(double delta)
	{
		if (VisualState is State.Spawning or State.Normal)
		{
			float dir = Mirrored ? 1 : -1;
			var time = (long)Time.Singleton.GetTicksMsec();
			var pos = Music.Singleton.SongDelay / 6f * (1 - (float)(time - StartsAt) / (EndsAt - StartsAt));
			Position = new Vector2(pos * dir, 0);
		}

		if (VisualState != State.Spawning)
			return;
		
		DrawColor = new Color(DrawColor.R, DrawColor.G, DrawColor.B, DrawColor.A + 1 * (float)delta);
		QueueRedraw();

		if (DrawColor.A >= 0.5f)
		{
			DrawColor = new Color(DrawColor.R, DrawColor.G, DrawColor.B, 0.5f);
			VisualState = State.Normal;
		}
	}

	public void Cleanup()
	{
		QueueFree();
	}
}
