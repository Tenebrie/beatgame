using Godot;
using System;

namespace Project;

[Tool]
public partial class CentralCircle : Control
{
	public override void _Draw()
	{
		DrawArc(new Vector2(0, 0), 50, 0, (float)Math.PI * 2, 50, new Color(255, 255, 255), 10, true);
	}
}
