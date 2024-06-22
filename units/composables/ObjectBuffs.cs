using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
namespace Project;

public partial class ObjectBuffs : ComposableScript
{
	public BuffUnitStatsVisitor State = new();

	readonly List<BaseBuff> Buffs = new();
	readonly List<(BaseBuff, int)> BeatTickingBuffs = new();

	public ObjectBuffs(BaseUnit parent) : base(parent) { }

	public override void _Ready()
	{
		base._Ready();
		Recalculate();
		Parent.ChildExitingTree += OnChildExitingTree;
		Music.Singleton.BeatTick += OnBeatTick;
	}

	public override void _ExitTree()
	{
		Parent.ChildExitingTree -= OnChildExitingTree;
		Music.Singleton.BeatTick -= OnBeatTick;
	}

	void OnChildExitingTree(Node child)
	{
		if (child is not BaseBuff buff)
			return;

		Buffs.Remove(buff);
		Recalculate();
	}

	void OnBeatTick(BeatTime time)
	{
		foreach (var buff in BeatTickingBuffs)
			buff.Item1.OnBeatTick(time, buff.Item2);
	}

	public void Add(BaseBuff buff)
	{
		if (buff.Settings.PreventStacking)
			RemoveAll(buff.GetType());
		if (buff.Settings.RefreshOthersWhenAdded)
			RefreshDuration(buff.GetType());

		buff.Parent = Parent;
		Buffs.Add(buff);
		Parent.AddChild(buff);
		Recalculate();
	}

	public bool Has<BuffClass>() where BuffClass : BaseBuff
	{
		return Buffs.Find(buff => buff is BuffClass) != null;
	}

	public int Stacks<BuffClass>() where BuffClass : BaseBuff
	{
		return Buffs.Count(buff => buff is BuffClass);
	}

	public void RefreshDuration(Type buffType)
	{
		var buffsToRefresh = Buffs.Where(buff => buff.GetType().IsInstanceOfType(buffType)).ToList();
		foreach (var buff in buffsToRefresh)
			buff.RefreshDuration();
	}

	public static void Remove(BaseBuff buff)
	{
		buff.QueueFree();
	}

	public void RemoveStacks<BuffClass>(int stacks) where BuffClass : BaseBuff
	{
		var buffs = Buffs.Where(buff => buff is BuffClass).Take(stacks).ToList();
		foreach (var buff in buffs)
			buff.QueueFree();
	}

	public void RemoveWithFlag(BaseBuff.Flag flags)
	{
		var buffsToRemove = Buffs.Where(buff => (buff.Flags & flags) > 0).ToList();
		foreach (var buff in buffsToRemove)
			buff.QueueFree();
	}

	public void RemoveAll(Type buffType)
	{
		var buffsToRemove = Buffs.Where(buff => buff.GetType() == buffType).ToList();
		foreach (var buff in buffsToRemove)
			buff.QueueFree();
	}

	public void RemoveAll<BuffClass>() where BuffClass : BaseBuff
	{
		var buffsToRemove = Buffs.Where(buff => buff is BuffClass).ToList();
		foreach (var buff in buffsToRemove)
			buff.QueueFree();
	}

	public void Recalculate()
	{
		BuffUnitStatsVisitor visitor = new();
		HashSet<Type> beatTickingBuffTypes = new();

		foreach (var buff in Buffs)
		{
			buff.ModifyUnit(visitor);
			if (buff.Settings.TicksOnBeat)
				beatTickingBuffTypes.Add(buff.GetType());
		}

		BeatTickingBuffs.Clear();
		foreach (var buffType in beatTickingBuffTypes)
		{
			var matchingBuffs = Buffs.Where(b => b.GetType() == buffType);
			BeatTickingBuffs.Add((matchingBuffs.First(), matchingBuffs.Count()));
		}

		Parent.Gravity = Parent.BaseGravity * visitor.GravityModifier;
		Parent.Health.SetMaxValue(Parent.Health.BaseMaximum + visitor.MaximumHealth);
		Parent.Mana.SetMaxValue(Parent.Mana.BaseMaximum + visitor.MaximumMana);
		State = visitor;
	}

	public BuffIncomingDamageVisitor ApplyIncomingDamageModifiers(ObjectResourceType type, float value, BaseUnit sourceUnit, BaseCast sourceCast)
	{
		var visitor = new BuffIncomingDamageVisitor
		{
			ResourceType = type,
			Value = value,
			BaseValue = value,
			SourceUnit = sourceUnit,
			SourceCast = sourceCast,
			Target = Parent,
		};
		foreach (var buff in Buffs)
		{
			buff.ModifyIncomingDamage(visitor);
		}

		visitor.Value *= State.PercentageDamageTaken.GetValueOrDefault(type);

		return visitor;
	}
	public BuffOutgoingDamageVisitor ApplyOutgoingDamageModifiers(ObjectResourceType type, float value, BaseUnit target)
	{
		var visitor = new BuffOutgoingDamageVisitor
		{
			ResourceType = type,
			Value = value,
			BaseValue = value,
			Target = target,
		};
		foreach (var buff in Buffs)
		{
			buff.ModifyOutgoingDamage(visitor);
		}
		return visitor;
	}
	public BuffIncomingRestorationVisitor ApplyIncomingRestorationModifiers(ObjectResourceType type, float value, BaseUnit sourceUnit, BaseCast sourceCast)
	{
		var visitor = new BuffIncomingRestorationVisitor
		{
			ResourceType = type,
			Value = value,
			BaseValue = value,
			SourceUnit = sourceUnit,
			SourceCast = sourceCast,
			Target = Parent,
		};
		foreach (var buff in Buffs)
		{
			buff.ModifyIncomingRestoration(visitor);
		}
		return visitor;
	}
	public BuffOutgoingRestorationVisitor ApplyOutgoingRestorationModifiers(ObjectResourceType type, float value, BaseUnit target)
	{
		var visitor = new BuffOutgoingRestorationVisitor
		{
			ResourceType = type,
			Value = value,
			BaseValue = value,
			Target = target,
		};
		foreach (var buff in Buffs)
		{
			buff.ModifyOutgoingRestoration(visitor);
		}
		return visitor;
	}
}
