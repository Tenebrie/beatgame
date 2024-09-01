using System;
using System.Linq;
using Godot;

namespace Project;

public partial class BaseBehaviour : Node
{
	protected BaseUnit Parent
	{
		get
		{
			var parent = GetParent();
			if (parent is BaseUnit unit)
				return unit;
			throw new Exception("This behaviour is not a child of BaseUnit");
		}
	}

	protected T GetComponent<T>() => CastUtils.GetComponent<T>(Parent);
}