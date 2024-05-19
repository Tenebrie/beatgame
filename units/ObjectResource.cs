using System;

namespace Project;

public class ObjectResource : ComposableScript
{
	private bool ready = false;
	private float current = 0;
	private float maximum = 0;

	public float Current
	{
		get => current;
	}
	public float Maximum
	{
		get => maximum;
	}

	public ObjectResourceType Type;

	public ObjectResource(BaseUnit parent, ObjectResourceType type, float max) : base(parent)
	{
		Type = type;
		current = max;
		maximum = max;
	}

	public override void _Ready()
	{
		ready = true;
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.ResourceChanged, Parent, Type.ToVariant(), current);
		SignalBus.Singleton.EmitSignal(SignalBus.SignalName.MaxResourceChanged, Parent, Type.ToVariant(), maximum);
	}

	public void Damage(float value, BaseUnit source = null)
	{
		current = Math.Max(0, current - value);

		if (ready)
			SignalBus.Singleton.EmitSignal(SignalBus.SignalName.ResourceChanged, Parent, Type.ToVariant(), current);
	}

	public void Restore(float value, BaseUnit source = null)
	{
		current = Math.Min(maximum, current + value);

		if (ready)
			SignalBus.Singleton.EmitSignal(SignalBus.SignalName.ResourceChanged, Parent, Type.ToVariant(), current);
	}

	public void SetMax(float value)
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
}