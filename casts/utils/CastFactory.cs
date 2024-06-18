using System;

namespace Project;

public class CastFactory
{
	public readonly Type CastType;
	public BaseCast.CastSettings Settings
	{
		get => Create(PlayerController.AllPlayers.Count > 0 ? PlayerController.AllPlayers[0] : null).Settings;

	}

	public CastFactory(Type prototype)
	{
		CastType = prototype;
	}

	public BaseCast Create(BaseUnit parent)
	{
		var cast = (BaseCast)Activator.CreateInstance(CastType, parent);
		if (parent != null)
			cast.PrepareSettings();
		return cast;
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

