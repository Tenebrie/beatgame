using System.Diagnostics;
using Godot;

namespace Project;
public partial class ResourceManager : Node
{
	public readonly static PackedScene UI_UnitCard = GD.Load<PackedScene>("uid://c2m17h2hubigh");

	static ResourceManager()
	{
	}
}