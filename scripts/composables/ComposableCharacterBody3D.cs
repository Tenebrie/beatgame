using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Project;

public abstract partial class ComposableCharacterBody3D : CharacterBody3D
{
	public ComposablesManager Composables;
	public ComposableCharacterBody3D()
	{
		Composables = new(this);
	}

	public class ComposablesManager
	{
		readonly Node3D Parent;

		public ComposablesManager(Node3D parent)
		{
			Parent = parent;
		}

		public ComposableScript Find(Func<ComposableScript, bool> predicate)
		{
			return Parent.GetChildren().Where(child => child is ComposableScript script && predicate(script)).Cast<ComposableScript>().FirstOrDefault();
		}
	}
}
