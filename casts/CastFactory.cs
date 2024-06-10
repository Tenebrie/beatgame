using System;

namespace Project;

public class CastFactory
{
	public readonly Type CastType;
	public readonly BaseCast.CastSettings Settings;

	public CastFactory(Type prototype)
	{
		CastType = prototype;
		Settings = Create(null).Settings;
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

