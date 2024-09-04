using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
namespace Project;

public partial class ObjectComponentLibrary : ComposableScript
{
	public ObjectComponentLibrary(BaseUnit parent) : base(parent) { }

    readonly Dictionary<Type, Node> componentCache = new();

	public bool GetCachedComponent<T>(out T outputNode) where T : Node
	{
		if (componentCache.TryGetValue(typeof (T), out var node) && IsInstanceValid(node)) {
			outputNode = (T)node;
			return true;
		}
		outputNode = null;
		return false;
	}

	public void CacheComponent<T>(T node) where T : Node
	{
		componentCache[typeof (T)] = node;
	}
}
