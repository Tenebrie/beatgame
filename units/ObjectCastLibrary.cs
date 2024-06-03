using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
namespace Project;

public partial class ObjectCastLibrary : ComposableScript
{
	public List<BaseCast> Abilities = new();

	public ObjectCastLibrary(BaseUnit parent) : base(parent) { }

	public CastT Register<CastT>(CastT cast) where CastT : BaseCast
	{
		Abilities.Add(cast);
		Parent.AddChild(cast);
		return cast;
	}
}
