using System;
using Godot;

namespace Project;

public partial class Message : PanelContainer
{
	float alpha = 1f;
	public string Body
	{
		get => BodyLabel.Text;
		set => BodyLabel.Text = value;
	}

	public Label BodyLabel;
	public float Duration
	{
		set => alpha = value;
	}

	public override void _EnterTree()
	{
		BodyLabel = GetNode<Label>("BodyLabel");
		BodyLabel.Text = Body;
	}

	public override void _Process(double delta)
	{
		alpha -= (float)delta;
		Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, Math.Min(1, alpha));
		if (alpha <= 0.00f)
			QueueFree();
	}
}
