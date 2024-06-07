using Godot;
using System;

namespace Project;

public partial class SkillForestUI : Control
{
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ToggleSkillForest"))
		{
			Visible = !Visible;
		}
	}
}
