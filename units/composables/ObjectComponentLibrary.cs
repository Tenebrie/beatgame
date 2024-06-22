using System;
using System.Diagnostics;
using Godot;
namespace Project;

public partial class ObjectComponentLibrary : ComposableScript
{
	public ObjectComponentLibrary(BaseUnit parent) : base(parent) { }

	public T Find<T>() where T : Node
	{
		foreach (var child in Parent.GetChildren())
		{
			if (child is not T correctChild)
				continue;

			return correctChild;
		}
		throw new Exception($"No child found.");
	}
}
