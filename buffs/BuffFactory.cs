using System;

namespace Project;

public class BuffFactory
{
	readonly Type BuffPrototype;

	public BuffFactory(Type prototype)
	{
		BuffPrototype = prototype;
	}

	public BaseBuff Create()
	{
		return (BaseBuff)Activator.CreateInstance(BuffPrototype);
	}

	public static BuffFactory Of<T>() where T : BaseBuff
	{
		return new BuffFactory(typeof(T));
	}
}

