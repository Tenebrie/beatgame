using Godot;
using System;

namespace Project;

public partial class SkillBindingNotification : Control
{
	float alpha = 2.0f;

	public Panel BodyLabel;

	public override void _EnterTree()
	{
		BodyLabel = GetNode<Panel>("Panel");
	}

	public override void _Process(double delta)
	{
		// TODO: New color
		alpha -= (float)delta;
		Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, Math.Min(1, alpha));
		if (alpha <= 0.00f)
			QueueFree();
	}
}
