using System;

namespace Project;

public class BuffFactory
{
	readonly Type BuffPrototype;
	public bool DescriptionOnly = false;
	public readonly BaseBuff.BuffSettings Settings;

	public BuffFactory(Type prototype)
	{
		BuffPrototype = prototype;
		Settings = Create().Settings;
	}

	public BaseBuff Create()
	{
		return (BaseBuff)Activator.CreateInstance(BuffPrototype);
	}

	public static BuffFactory Of<T>(bool descriptionOnly = false) where T : BaseBuff
	{
		return new BuffFactory(typeof(T))
		{
			DescriptionOnly = descriptionOnly
		};
	}
}

