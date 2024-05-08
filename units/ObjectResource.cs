using System;
using System.Diagnostics;
namespace Project;
public class ObjectResource : ComposableScript
{
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
		SignalBus.GetInstance(Parent).EmitSignal(SignalBus.SignalName.ResourceChanged, Parent, Type.ToVariant(), current);
		SignalBus.GetInstance(Parent).EmitSignal(SignalBus.SignalName.MaxResourceChanged, Parent, Type.ToVariant(), maximum);
	}

	public void Damage(float value, BaseUnit source = null)
	{
		current = Math.Max(0, current - value);
		SignalBus.GetInstance(Parent).EmitSignal(SignalBus.SignalName.ResourceChanged, Parent, Type.ToVariant(), current);
	}

	public void Restore(float value, BaseUnit source = null)
	{
		current = Math.Min(maximum, current + value);
		SignalBus.GetInstance(Parent).EmitSignal(SignalBus.SignalName.ResourceChanged, Parent, Type.ToVariant(), current);
	}
}