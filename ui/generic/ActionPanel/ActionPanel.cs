using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Project;
public partial class ActionPanel : HBoxContainer
{
	public override void _EnterTree()
	{
		foreach (var child in GetChildren())
			RemoveChild(child);
	}

	public override void _Ready()
	{
		var c1 = new ActionButton()
		{
			Label = "1",
			ActionName = "Cast1",
		};
		var c2 = new ActionButton()
		{
			Label = "2",
			ActionName = "Cast2",
		};
		var c3 = new ActionButton()
		{
			Label = "3",
			ActionName = "Cast3",
			IsDisabled = true,
		};
		var c4 = new ActionButton()
		{
			Label = "4",
			ActionName = "Cast4",
			IsDisabled = true,
		};
		var sc1 = new ActionButton()
		{
			Label = "S1",
			ActionName = "ShiftCast1",
		};
		var sc2 = new ActionButton()
		{
			Label = "S2",
			ActionName = "ShiftCast2",
		};
		var sc3 = new ActionButton()
		{
			Label = "S3",
			ActionName = "ShiftCast3",
			IsDisabled = true,
		};
		var sc4 = new ActionButton()
		{
			Label = "S4",
			ActionName = "ShiftCast4",
			IsDisabled = true,
		};

		List<ActionButton> buttons = new() { c1, c2, c3, c4, sc1, sc2, sc3, sc4 };
		var instances = buttons.Select(btn =>
		{
			var instance = Lib.Scene(Lib.UI.ActionButton).Instantiate<ActionButton>();
			instance.Label = btn.Label;
			instance.ActionName = btn.ActionName;
			instance.IsDisabled = btn.IsDisabled;
			return instance;
		});

		foreach (var instance in instances)
		{
			AddChild(instance);
		}
	}

	public override void _Process(double delta)
	{
	}
}
