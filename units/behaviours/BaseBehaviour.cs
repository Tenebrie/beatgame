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
			throw new System.Exception("This behaviour is not a child of BaseUnit");
		}
	}
}