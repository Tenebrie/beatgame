using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

	public CastT Get<CastT>() where CastT : BaseCast
	{
		var cast = Casts.Where(cast => cast is CastT).FirstOrDefault() ?? throw new Exception("Unable to find cast");
		return (CastT)cast;
	}
}
