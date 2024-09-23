using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Project;

[GlobalClass, Icon("SpringArm3D")]
public partial class BetterSpringArm3D : Node3D
{
	/**
	 * The root node the collisions are checked from
	 */
	[Export] public Node3D RootNode;
	/**
	 * The height keeping parent node
	 */
	[Export] public Node3D HeightNode;
	/**
	 * The length of the spring, or the distance the camera will try to keep from the target
	 */
	[Export] public float SpringLength = 1;
	/**
	 * The radius of a virtual sphere the arm is projecting to check for collisions
	 */
	[Export] public float Margin = 0.1f;
	/**
	 * The layers the arm will collide with
	 */
	[Export] public Raycast.Layer CollisionLayers = Raycast.Layer.Walls | Raycast.Layer.Floors | Raycast.Layer.Ceilings;

	Camera3D camera;
	Node3D cameraPlaceholder;
	ShapeCast3D shapeCast;
	float rememberedCameraHeight = 0;

	public override void _Ready()
	{
		camera = this.GetComponent<Camera3D>();
		cameraPlaceholder = new Node3D();
		AddChild(cameraPlaceholder);

		shapeCast = new ShapeCast3D
		{
			Shape = new SphereShape3D { Radius = Margin },
			CollisionMask = (uint)CollisionLayers,
		};
		AddChild(shapeCast);
		shapeCast.ForceShapecastUpdate();
	}

	public override void _Process(double delta)
	{
		var hasCollision = GetFinalCollisionPosition(out var hitPos);
		if (hasCollision)
		{
			SetCameraPosition((float)delta, hitPos, isColliding: true);
			return;
		}

		SetCameraPosition((float)delta, ToGlobal(cameraPlaceholder.Position), isColliding: false);
	}

	bool GetFinalCollisionPosition(out Vector3 hitPos)
	{
		cameraPlaceholder.Position = new Vector3(0, 0, SpringLength + Margin);
		var rootPosition = RootNode.GlobalPosition;
		var softHit = GetCollisionPosition(rootPosition, cameraPlaceholder.GlobalPosition, out var softHitPosition);

		if (softHit)
		{
			hitPos = softHitPosition;
			return true;
		}

		hitPos = Vector3.Zero;
		return false;
	}

	bool GetCollisionPosition(Vector3 from, Vector3 to, out Vector3 hitPos)
	{
		shapeCast.GlobalPosition = from;
		shapeCast.TargetPosition = shapeCast.ToLocal(to);

		shapeCast.ForceShapecastUpdate();
		if (shapeCast.GetCollisionCount() == 0)
		{
			hitPos = Vector3.Zero;
			return false;
		}

		var fraction = shapeCast.GetClosestCollisionSafeFraction();
		hitPos = from.MoveToward(to, (from - to).Length() * fraction);
		return true;
	}

	void SetCameraPosition(float delta, Vector3 globalTarget, bool isColliding)
	{
		var rootPosition = RootNode.GlobalPosition;
		if (isColliding && rootPosition.DistanceSquaredTo(globalTarget) < rootPosition.DistanceSquaredTo(camera.GlobalPosition))
		{
			camera.GlobalPosition = globalTarget;
			return;
		}

		camera.GlobalPosition = camera.GlobalPosition.MoveToward(globalTarget, 5 * delta);
	}
}