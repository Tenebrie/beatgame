using Godot;
namespace Project;

public static class CastUtils
{
	private static readonly PackedScene GroundAreaCircleScene = GD.Load<PackedScene>("res://effects/GroundAreaCircle/GroundAreaCircle.tscn");

	public static GroundAreaCircle CreateGroundCircularArea(this Node node, Vector3 point)
	{
		var circle = GroundAreaCircleScene.Instantiate<GroundAreaCircle>();
		circle.Position = point;
		node.GetTree().Root.AddChild(circle);
		return circle;
	}
}