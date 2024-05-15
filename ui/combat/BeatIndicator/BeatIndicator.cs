using Godot;
using System;

namespace Project;

public partial class BeatIndicator : Control
{
	public MeshInstance2D MainBeat;
	public override void _Ready()
	{
		MainBeat = GetNode<MeshInstance2D>("MainBeat");
		Music.Singleton.BeatTimer.Timeout += OnBeat;
	}

	public void OnBeat()
	{
		MainBeat.Scale = new Vector2(1, 1);
	}

	public override void _Process(double delta)
	{
		MainBeat.Scale = new Vector2(MainBeat.Scale.X - .5f * (float)delta, MainBeat.Scale.Y - .5f * (float)delta);
	}
}
