using Godot;

namespace Project;

public partial class BaseBehaviour<ParentT> : Node where ParentT : BaseUnit
{
	protected ParentT Parent
	{
		get
		{
			var parent = GetParent();
			if (parent is ParentT unit)
				return unit;
			throw new System.Exception("This behaviour is not a child of expected parent");
		}
	}
}