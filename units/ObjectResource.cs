using System;
using System.Collections.Generic;

namespace Project;

public class ObjectResource : ComposableScript
{
	private bool ready = false;
	private float current = 0;
	private float maximum = 0;
	private float baseMaximum = 0;

	public float Current
	{
		get => current;
	}
	public float Maximum
	{
		get => maximum;
	}
	public float BaseMaximum
	{
		get => baseMaximum;
	}

	public ObjectResourceType Type;

	public ObjectResource(BaseUnit parent, ObjectResourceType type, float max) : base(parent)
	{
		Type = type;
		current = max;
		maximum = max;
		baseMaximum = max;
	}

	public override void _Ready()
	{
		ready = true;
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.ResourceChanged, Parent, Type.ToVariant(), current);
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.MaxResourceChanged, Parent, Type.ToVariant(), maximum);
	}

	public void Damage(float originalValue, BaseCast sourceCast)
	{
		Damage(originalValue, sourceCast.Parent, sourceCast);
	}
	public void Damage(float originalValue, BaseUnit sourceUnit)
	{
		Damage(originalValue, sourceUnit, null);
	}
	public void Damage(float originalValue, BaseUnit sourceUnit, BaseCast sourceCast)
	{
		BuffOutgoingDamageVisitor visitor = null;
		if (sourceUnit != null)
		{
			visitor = sourceUnit.Buffs.ApplyOutgoingDamageModifiers(Type, originalValue, Parent);
			if (visitor.Target != Parent)
				throw new NotImplementedException("Redirecting damage target is not implemented");
			if (visitor.ResourceType != Type)
				throw new NotImplementedException("Changing resource type is not implemented");
		}

		var result = Parent.Buffs.ApplyIncomingDamageModifiers(Type, visitor?.Value ?? originalValue, sourceUnit, sourceCast);
		if (result.Target != Parent)
			throw new NotImplementedException("Redirecting damage target is not implemented");
		if (result.ResourceType != Type)
			throw new NotImplementedException("Changing resource type is not implemented");

		var value = result.Value;
		current = Math.Max(0, current - value);

		if (ready)
			SignalBus.Singleton.EmitSignal(SignalBus.SignalName.ResourceChanged, Parent, Type.ToVariant(), current);
	}

	public void Restore(float originalValue, BaseCast sourceCast)
	{
		Restore(originalValue, sourceCast.Parent, sourceCast);
	}
	public void Restore(float originalValue, BaseUnit sourceUnit)
	{
		Restore(originalValue, sourceUnit, null);
	}
	public void Restore(float originalValue, BaseUnit sourceUnit, BaseCast sourceCast)
	{
		BuffOutgoingRestorationVisitor visitor = null;
		if (sourceUnit != null)
		{
			visitor = sourceUnit.Buffs.ApplyOutgoingRestorationModifiers(Type, originalValue, Parent);
			if (visitor.Target != Parent)
				throw new NotImplementedException("Redirecting restoration target is not implemented");
			if (visitor.ResourceType != Type)
				throw new NotImplementedException("Changing resource type is not implemented");
		}

		var result = Parent.Buffs.ApplyIncomingRestorationModifiers(Type, visitor?.Value ?? originalValue, sourceUnit, sourceCast);
		if (result.Target != Parent)
			throw new NotImplementedException("Redirecting restoration target is not implemented");
		if (result.ResourceType != Type)
			throw new NotImplementedException("Changing resource type is not implemented");

		var value = result.Value;
		current = Math.Min(maximum, current + value);

		if (ready)
			SignalBus.Singleton.EmitSignal(SignalBus.SignalName.ResourceChanged, Parent, Type.ToVariant(), current);
	}

	public void SetMaxValue(float value)
	{
		if (value == maximum)
			return;

		var floorValue = Type == ObjectResourceType.Health ? 1 : 0;

		value = Math.Max(floorValue, value);
		var delta = value - maximum;
		maximum = value;
		current += delta;
		if (ready)
		{
			SignalBus.Singleton.EmitSignal(SignalBus.SignalName.ResourceChanged, Parent, Type.ToVariant(), current);
			SignalBus.Singleton.EmitSignal(SignalBus.SignalName.MaxResourceChanged, Parent, Type.ToVariant(), maximum);
		}
	}

	public void SetBaseMaxValue(float value)
	{
		SetMaxValue(value);
		baseMaximum = value;
	}
}