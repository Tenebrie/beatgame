using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
namespace Project;

public partial class ObjectCastLibrary : ComposableScript
{
	public List<BaseCast> Casts = new();

	public ObjectCastLibrary(BaseUnit parent) : base(parent) { }

	public CastT Register<CastT>(CastT cast) where CastT : BaseCast
	{
		Casts.Add(cast);
		Parent.AddChild(cast);
		return cast;
	}

	public CastT Unregister<CastT>(CastT cast) where CastT : BaseCast
	{
		Casts.Remove(cast);
		cast.QueueFree();
		return cast;
	}
}
