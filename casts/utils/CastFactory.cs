using System;

namespace Project;

public class CastFactory
{
	public readonly Type CastType;
	public BaseCast.CastSettings Settings
	{
		get => Create(null).Settings;
	}

	public CastFactory(Type prototype)
	{
		CastType = prototype;
	}

	public BaseCast Create(BaseUnit parent)
	{
		return (BaseCast)Activator.CreateInstance(CastType, parent);
	}

	public static CastFactory Of<T>() where T : BaseCast
	{
		return new CastFactory(typeof(T));
	}

	public static CastFactory Of(Type prototype)
	{
		return new CastFactory(prototype);
	}
}

