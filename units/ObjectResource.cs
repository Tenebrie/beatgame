using System;
using System.Collections.Generic;

namespace Project;

public class ObjectResource : ComposableScript
{
	private bool ready = false;
	private float minimum = 0;
	private float current = 0;
	private float maximum = 0;
	private float baseMaximum = 0;
	private float regen = 0;
	private float regenPool = 0;

	public float Minimum
	{
		get => minimum;
	}
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
	public float Regeneration
	{
		get => regen;
		set => regen = value;
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

	public override void _Process(double delta)
	{
		if (regen == 0)
			return;

		var resourceMissing = maximum - current;
		if (resourceMissing == 0)
		{
			regenPool = 0;
			return;
		}

		regenPool += regen * (float)delta * Music.Singleton.BeatsPerSecond;
		if (regenPool >= 1f)
		{
			current = Math.Min(maximum, current + regenPool);
			regenPool = 0;
			SignalBus.Singleton.EmitSignal(SignalBus.SignalName.ResourceRegenerated, Parent, Type.ToVariant(), current);
		}
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
		current = Math.Max(minimum, current - value);

		if (ready)
		{
			SignalBus.Singleton.EmitSignal(SignalBus.SignalName.DamageTaken, result);
			SignalBus.Singleton.EmitSignal(SignalBus.SignalName.ResourceChanged, Parent, Type.ToVariant(), current);
		}
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

	public void SetMinValue(float value)
	{
		if (value == minimum)
			return;

		minimum = value;
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

	public static Dictionary<ObjectResourceType, T> MakeDictionary<T>(T defaultValue)
	{
		var dict = new Dictionary<ObjectResourceType, T>();
		foreach (ObjectResourceType resource in (ObjectResourceType[])Enum.GetValues(typeof(ObjectResourceType)))
		{
			dict[resource] = defaultValue;
		}
		return dict;
	}
}