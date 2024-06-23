using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project;

public partial class BuffContainer : Control
{
	[Export]
	FlowContainer container;

	BaseUnit trackedUnit;
	Dictionary<Type, BuffButton> buttonDict = new();

	public override void _Ready()
	{
		var children = container.GetChildren();
		foreach (var child in children)
			child.QueueFree();
	}

	public void TrackUnit(BaseUnit unit)
	{
		trackedUnit = unit;
		trackedUnit.Buffs.BuffAdded += OnBuffAdded;
		trackedUnit.Buffs.BuffRemoved += OnBuffRemoved;
	}

	public void UntrackUnit()
	{
		trackedUnit.Buffs.BuffAdded -= OnBuffAdded;
		trackedUnit.Buffs.BuffRemoved -= OnBuffRemoved;
	}

	void OnBuffAdded(BaseBuff buff)
	{
		if (buff.Settings.Hidden)
			return;

		var isPresent = buttonDict.TryGetValue(buff.GetType(), out var button);

		if (!isPresent)
		{
			button = Lib.LoadScene(Lib.UI.BuffButton).Instantiate<BuffButton>();
			container.AddChild(button);
			button.AssignBuff(buff);
			buttonDict.Add(buff.GetType(), button);
		}
		else
			button.AssignBuff(buff);
	}

	void OnBuffRemoved(BaseBuff buff)
	{
		var isPresent = buttonDict.TryGetValue(buff.GetType(), out var button);
		if (!isPresent)
			return;

		var stacks = button.UpdateStacks();
		if (stacks == 0)
		{
			button.QueueFree();
			buttonDict.Remove(buff.GetType());
			return;
		}

		if (button.AssociatedBuff == buff)
			button.AssignBuff(buff.Parent.Buffs.Find(buff.GetType()));
	}
}
