using System;

namespace Project;

public class CastFactory
{
	readonly Type CastPrototype;

	public CastFactory(Type prototype)
	{
		CastPrototype = prototype;
	}

	public BaseCast Create(BaseUnit parent)
	{
		return (BaseCast)Activator.CreateInstance(CastPrototype, parent);
	}

	public static CastFactory Of<T>() where T : BaseCast
	{
		return new CastFactory(typeof(T));
	}
}

