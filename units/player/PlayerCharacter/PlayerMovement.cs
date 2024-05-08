using Godot;
using System;
using System.Diagnostics;

namespace Project;

public partial class PlayerMovement : ComposableScript
{
	private NodePath _mainCameraPath = null;
	private NodePath _baseCameraPath = null;

	Camera3D mainCamera;

	// Ignores soft rotation
	Camera3D baseCamera;

	bool softCameraMoving = false;
	Vector2 softCameraMoveStart;
	bool hardCameraMoving = false;
	Vector2 hardCameraMoveStart;

	float cameraDistance = 1.5f;
	float cameraHeight = 1;

	float jumpCount = 0;

	new readonly PlayerController Parent;

	public PlayerMovement(BaseUnit parent, NodePath mainCameraPath, NodePath baseCameraPath) : base(parent)
	{
		Parent = parent as PlayerController;
		_mainCameraPath = mainCameraPath;
		_baseCameraPath = baseCameraPath;
	}

	public override void _Ready()
	{
		mainCamera = Parent.GetNode<Camera3D>(_mainCameraPath);
		baseCamera = Parent.GetNode<Camera3D>(_baseCameraPath);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("SoftCameraMove"))
		{
			softCameraMoving = true;
			softCameraMoveStart = GetWindow().GetMousePosition();
		}
		if (@event.IsActionReleased("SoftCameraMove"))
		{
			softCameraMoving = false;
			baseCamera.Position = mainCamera.Position;
		}

		if (@event.IsActionPressed("HardCameraMove"))
		{
			hardCameraMoveStart = GetWindow().GetMousePosition();
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
			if (Parent.Grounded)
			{
				jumpCount = 1;
				Parent.Velocity = new Vector3(Velocity.X, 3f, Velocity.Z);
			}
			else
			{
				jumpCount = 2;
				Parent.Velocity = new Vector3(Velocity.X, 3.5f, Velocity.Z);
			}
		}
	}

	public override void _Process(double delta)
	{
		ProcessMovement(delta);
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
		var forwardVector = -GlobalTransform.Basis.Z;
		var rightVector = GlobalTransform.Basis.X;

		Parent.MoveAndCollide(new Vector3(
			x: forwardVector.X * movementVector.Y + rightVector.X * movementVector.X,
			y: 0,
			z: forwardVector.Z * movementVector.Y + rightVector.Z * movementVector.X
		));

		var rotationSpeed = 2;
		if (Input.IsActionPressed("TurnLeft") && !Input.IsActionPressed("HardCameraMove"))
		{
			Parent.Rotate(Vector3.Up, rotationSpeed * (float)delta);
		}
		if (Input.IsActionPressed("TurnRight") && !Input.IsActionPressed("HardCameraMove"))
		{
			Parent.Rotate(Vector3.Up, -rotationSpeed * (float)delta);
		}
	}

	private void ProcessCamera(double delta)
	{
		var mousePos = GetWindow().GetMousePosition();
		if (Input.IsActionPressed("HardCameraMove"))
		{
			var mouseDelta = mousePos - hardCameraMoveStart;
			Input.WarpMouse(hardCameraMoveStart);
			Parent.Rotate(Vector3.Up, -mouseDelta.X / 500);
			cameraHeight = Math.Max(0, Math.Min(cameraHeight + mouseDelta.Y / 500, 3));
		}

		var forward = -Parent.GlobalTransform.Basis.Z;

		var targetCameraPosition = Position - forward * cameraDistance + new Vector3(0, cameraHeight, 0);

		var snappingSpeed = hardCameraMoving ? 20 : 10;

		baseCamera.Position += (float)delta * snappingSpeed * (targetCameraPosition - baseCamera.Position);

		var verticalOffset = new Vector3(0, 0.5f, 0);
		mainCamera.Position = baseCamera.Position;
		mainCamera.LookAt(Position + verticalOffset);
	}

	public void ResetJumpCount()
	{
		jumpCount = 0;
	}
}
