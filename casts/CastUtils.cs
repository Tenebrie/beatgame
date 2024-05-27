using Godot;
namespace Project;

public static class CastUtils
{
	private static readonly PackedScene GroundAreaCircleScene = GD.Load<PackedScene>("res://effects/GroundAreaCircle/GroundAreaCircle.tscn");
	private static readonly PackedScene PackedGroundAreaRect = GD.Load<PackedScene>("res://effects/GroundAreaRect/GroundAreaRect.tscn");

	public static GroundAreaCircle CreateGroundCircularArea(this Node node, Vector3 point)
	{
		var circle = GroundAreaCircleScene.Instantiate<GroundAreaCircle>();
		circle.Position = point;
		node.GetTree().Root.AddChild(circle);
		return circle;
	}

	public static GroundAreaRect CreateGroundRectangularArea(this Node node, Vector3 point)
	{
		var rect = PackedGroundAreaRect.Instantiate<GroundAreaRect>();
		rect.Position = point;
		node.GetTree().Root.AddChild(rect);
		return rect;
	}
}