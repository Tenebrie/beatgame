using Godot;
using System;
using System.Diagnostics;

namespace Project;

public partial class PlayerMovement : Node
{
	[Export] public NodePath _mainCameraPath = null;
	[Export] public NodePath _baseCameraPath = null;

	Camera3D mainCamera;

	// Ignores soft rotation
	Camera3D baseCamera;

	bool softCameraMoving = false;
	Vector2 softCameraMoveStart;
	bool hardCameraMoving = false;
	Vector2 hardCameraMoveStart;

	float cameraDistance = 1.5f;
	float cameraHeight = 1;

	Vector3 inertia = new();

	const float terminalVelocity = -30f;
	float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	float jumpCount = 0;

	PlayerController parent;

	public override void _Ready()
	{
		parent = GetParent<PlayerController>();
		mainCamera = GetNode<Camera3D>(_mainCameraPath);
		baseCamera = GetNode<Camera3D>(_baseCameraPath);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("SoftCameraMove"))
		{
			softCameraMoving = true;
			softCameraMoveStart = GetViewport().GetMousePosition();
		}
		if (@event.IsActionReleased("SoftCameraMove"))
		{
			softCameraMoving = false;
			baseCamera.Position = mainCamera.Position;
		}

		if (@event.IsActionPressed("HardCameraMove"))
		{
			hardCameraMoving = true;
			hardCameraMoveStart = GetViewport().GetMousePosition();
			Input.MouseMode = Input.MouseModeEnum.Hidden;
		}
		if (@event.IsActionReleased("HardCameraMove"))
		{
			hardCameraMoving = false;
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}

		if (@event.IsActionPressed("ZoomIn"))
		{
			cameraDistance -= 0.25f;
		}
		if (@event.IsActionPressed("ZoomOut"))
		{
			cameraDistance += 0.25f;
		}

		if (@event.IsActionPressed("Jump") && jumpCount < 2)
		{
			inertia.Y = 3f;
			jumpCount += 1;
		}
	}

	public override void _Process(double delta)
	{
		ProcessMovement(delta);
		ProcessGravity(delta);
		ProcessCamera(delta);
	}

	private void ProcessMovement(double delta)
	{
		var movementSpeed = 2.5f * (float)delta;

		float movementForward = 0;
		if (Input.IsActionPressed("MoveForward"))
		{
			movementForward += 1;
		}
		if (Input.IsActionPressed("MoveBackward"))
		{
			movementForward -= 1;
		}
		if (Input.IsActionPressed("SoftCameraMove") && Input.IsActionPressed("HardCameraMove"))
		{
			movementForward += 1;
		}

		float movementRight = 0;
		if (Input.IsActionPressed("MoveRight"))
		{
			movementRight += 1;
		}
		if (Input.IsActionPressed("MoveLeft"))
		{
			movementRight -= 1;
		}
		if (Input.IsActionPressed("TurnLeft") && Input.IsActionPressed("HardCameraMove"))
		{
			movementRight -= 1;
		}
		if (Input.IsActionPressed("TurnRight") && Input.IsActionPressed("HardCameraMove"))
		{
			movementRight += 1;
		}

		var movementVector = new Vector2(movementRight, movementForward).Normalized() * movementSpeed;
		var forwardVector = -parent.GlobalTransform.Basis.Z;
		var rightVector = parent.GlobalTransform.Basis.X;

		parent.MoveAndCollide(new Vector3(
			x: forwardVector.X * movementVector.Y + rightVector.X * movementVector.X,
			y: 0,
			z: forwardVector.Z * movementVector.Y + rightVector.Z * movementVector.X
		));

		var rotationSpeed = 2;
		if (Input.IsActionPressed("TurnLeft") && !Input.IsActionPressed("HardCameraMove"))
		{
			parent.Rotate(Vector3.Up, rotationSpeed * (float)delta);
		}
		if (Input.IsActionPressed("TurnRight") && !Input.IsActionPressed("HardCameraMove"))
		{
			parent.Rotate(Vector3.Up, -rotationSpeed * (float)delta);
		}
	}

	private void ProcessGravity(double delta)
	{
		inertia.Y = Math.Max(terminalVelocity, inertia.Y - gravity * (float)delta);

		var collision = parent.MoveAndCollide(new Vector3(
			x: 0,
			y: inertia.Y * (float)delta,
			z: 0
		));

		var grounded = collision != null;

		if (grounded)
		{
			inertia.Y = 0;
			jumpCount = 0;
		}
	}

	private void ProcessCamera(double delta)
	{
		var mousePos = parent.GetViewport().GetMousePosition();
		if (Input.IsActionPressed("HardCameraMove"))
		{
			var mouseDelta = mousePos - hardCameraMoveStart;
			Input.WarpMouse(hardCameraMoveStart);
			parent.Rotate(Vector3.Up, -mouseDelta.X / 500);
			cameraHeight = Math.Max(0, Math.Min(cameraHeight + mouseDelta.Y / 500, 3));
		}

		var forward = -parent.GlobalTransform.Basis.Z;

		var targetCameraPosition = parent.Position - forward * cameraDistance + new Vector3(0, cameraHeight, 0);

		var snappingSpeed = hardCameraMoving ? 20 : 10;

		baseCamera.Position = baseCamera.Position + (float)delta * snappingSpeed * (targetCameraPosition - baseCamera.Position);

		var verticalOffset = new Vector3(0, 0.5f, 0);
		mainCamera.Position = baseCamera.Position;
		mainCamera.LookAt(parent.Position + verticalOffset);
	}
}
