using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
namespace Project;

public partial class ObjectBuffs : ComposableScript
{
	public BuffVisitor State = new();

	readonly List<BaseBuff> Buffs = new();

	public ObjectBuffs(BaseUnit parent) : base(parent) { }

	public override void _Ready()
	{
		base._Ready();
		Recalculate();
	}

	public void Add(BaseBuff buff)
	{
		buff.Parent = Parent;
		Buffs.Add(buff);
		Parent.AddChild(buff);
		Recalculate();
	}

	public bool Has<BuffClass>() where BuffClass : BaseBuff
	{
		return Buffs.Find(buff => buff is BuffClass) != null;
	}

	public void Remove(BaseBuff buff)
	{
		Buffs.Remove(buff);
		buff.QueueFree();
		Recalculate();
	}

	public void RemoveAll<BuffClass>() where BuffClass : BaseBuff
	{
		var buffsToRemove = Buffs.Where(buff => buff is BuffClass).ToList();
		foreach (var buff in buffsToRemove)
		{
			Buffs.Remove(buff);
			buff.QueueFree();
		}
		Recalculate();
	}

	public void Recalculate()
	{
		BuffVisitor visitor = new();

		foreach (var buff in Buffs)
			buff.Visit(visitor);

		Parent.Gravity = Parent.BaseGravity * visitor.GravityModifier;
		State = visitor;
	}
}
