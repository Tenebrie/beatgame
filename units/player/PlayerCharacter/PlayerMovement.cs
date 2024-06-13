using Godot;
using System;
using System.Diagnostics;

namespace Project;

public partial class PlayerMovement : ComposableScript
{
	bool softCameraPreMoving = false;
	bool softCameraMoving = false;
	Vector2 softCameraMoveStart;
	bool hardCameraMoving = false;
	Vector2 hardCameraMoveStart;

	private Node3D horizontalCameraPivot;
	private Node3D verticalCameraPivot;
	private SpringArm3D springArm;

	bool autorunEnabled = false;
	float cameraDistance = 1.5f;

	float jumpCount = 0;

	new readonly PlayerController Parent;

	public PlayerMovement(BaseUnit parent) : base(parent)
	{
		Parent = parent as PlayerController;
	}

	public override void _Ready()
	{
		horizontalCameraPivot = Parent.GetNode<Node3D>("HCameraPivot");
		verticalCameraPivot = Parent.GetNode<Node3D>("HCameraPivot/VCameraPivot");
		springArm = Parent.GetNode<SpringArm3D>("HCameraPivot/VCameraPivot/SpringArm3D");
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("SoftCameraMove"))
		{
			softCameraPreMoving = true;
			softCameraMoveStart = GetWindow().GetMousePosition();
		}
		if (@event.IsActionReleased("SoftCameraMove"))
		{
			softCameraPreMoving = false;
			softCameraMoving = false;
			if (!hardCameraMoving)
				Input.MouseMode = Input.MouseModeEnum.Visible;
		}

		if (@event.IsActionPressed("HardCameraMove"))
		{
			hardCameraMoving = true;
			hardCameraMoveStart = GetWindow().GetMousePosition();
			Input.MouseMode = Input.MouseModeEnum.Hidden;
			Parent.Rotate(Vector3.Up, horizontalCameraPivot.Rotation.Y);
			RotateCameraHorizontal(-horizontalCameraPivot.Rotation.Y);
		}
		if (@event.IsActionReleased("HardCameraMove"))
		{
			hardCameraMoving = false;
			if (!softCameraMoving)
				Input.MouseMode = Input.MouseModeEnum.Visible;
		}

		if (@event.IsActionPressed("ZoomIn"))
		{
			cameraDistance = Math.Max(0, cameraDistance - 0.15f);
		}
		if (@event.IsActionPressed("ZoomOut"))
		{
			cameraDistance = Math.Min(4, cameraDistance + 0.15f);
		}

		if (@event.IsActionPressed("Jump") && jumpCount < 2)
		{
			if (Parent.Grounded)
			{
				jumpCount = 1;
				Parent.Velocity = new Vector3(Velocity.X, 4f, Velocity.Z);
			}
			else
			{
				jumpCount = 2;
				Parent.Velocity = new Vector3(Velocity.X, 5f, Velocity.Z);
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
		var movementSpeed = 2.5f * Parent.Buffs.State.MoveSpeedPercentage;

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

		if (Input.IsActionJustPressed("Autorun"))
			autorunEnabled = !autorunEnabled;

		if (movementForward != 0 || movementRight != 0)
			autorunEnabled = false;

		if (autorunEnabled)
			movementForward = 1;

		var movementVector = new Vector2(movementRight, movementForward).Normalized() * movementSpeed;
		var forwardVector = -GlobalTransform.Basis.Z;
		var rightVector = GlobalTransform.Basis.X;

		var velocity = Parent.Velocity;
		Parent.Velocity = new Vector3(
			x: forwardVector.X * movementVector.Y + rightVector.X * movementVector.X,
			y: 0.001f,
			z: forwardVector.Z * movementVector.Y + rightVector.Z * movementVector.X
		);

		Parent.MoveAndSlide();

		Parent.Velocity = velocity;

		var rotationSpeed = 2; // radians per second
		if (Input.IsActionPressed("TurnLeft") && !Input.IsActionPressed("HardCameraMove"))
		{
			var rotation = rotationSpeed * (float)delta;
			Parent.Rotate(Vector3.Up, rotation);
			if (Input.IsActionPressed("SoftCameraMove"))
				RotateCameraHorizontal(-rotation);
		}
		if (Input.IsActionPressed("TurnRight") && !Input.IsActionPressed("HardCameraMove"))
		{
			var rotation = -rotationSpeed * (float)delta;
			Parent.Rotate(Vector3.Up, rotation);
			if (Input.IsActionPressed("SoftCameraMove"))
				RotateCameraHorizontal(-rotation);
		}

		if (!Input.IsActionPressed("SoftCameraMove") && !Input.IsActionPressed("HardCameraMove") && movementVector.Length() > 0)
		{
			var unrotationSpeed = 3f; // radians per second

			var rotation = -Math.Sign(horizontalCameraPivot.Rotation.Y) * unrotationSpeed * (float)delta;
			if (Math.Abs(horizontalCameraPivot.Rotation.Y) <= 0.01f)
				rotation = -horizontalCameraPivot.Rotation.Y;
			RotateCameraHorizontal(rotation, true);
		}

		// If moving, release the casting spell
		if ((movementForward != 0 || movementRight != 0) && !Parent.Buffs.Has<BuffCastWhileMoving>())
			Parent.Spellcasting.ReleaseCurrentCastingSpell();
	}

	private void ProcessCamera(double delta)
	{
		var mousePos = GetWindow().GetMousePosition();
		if (hardCameraMoving)
		{
			var mouseDelta = mousePos - hardCameraMoveStart;
			Input.WarpMouse(hardCameraMoveStart * GetTree().Root.ContentScaleFactor);
			Parent.Rotate(Vector3.Up, -mouseDelta.X / 500);
			var rotation = (float)Math.Min(Math.PI / 4, Math.Max(-Math.PI / 2 + 0.01f, verticalCameraPivot.Rotation.X - mouseDelta.Y / 500));
			verticalCameraPivot.Rotation = new Vector3(rotation, 0, 0);
		}
		else if (softCameraPreMoving)
		{
			var mouseDelta = mousePos - softCameraMoveStart;
			if (mouseDelta.Length() > 10)
			{
				softCameraPreMoving = false;
				softCameraMoving = true;
				Input.MouseMode = Input.MouseModeEnum.Hidden;
			}
		}
		else if (softCameraMoving)
		{
			var mouseDelta = mousePos - softCameraMoveStart;
			Input.WarpMouse(softCameraMoveStart * GetTree().Root.ContentScaleFactor);
			RotateCameraHorizontal(-mouseDelta.X / 500);
			var verticalRotation = (float)Math.Min(Math.PI / 4, Math.Max(-Math.PI / 2 + 0.01f, verticalCameraPivot.Rotation.X - mouseDelta.Y / 500));
			verticalCameraPivot.Rotation = new Vector3(verticalRotation, 0, 0);
		}

		var effectiveHeight = Preferences.Singleton.CameraHeight + 0.25f; // TODO: Should be generic 'unit center height'
		var minSnapDistance = 1.5f;
		var maxSnapDistance = 2.5f;
		if (cameraDistance < minSnapDistance)
			effectiveHeight -= (1 - (cameraDistance / minSnapDistance)) * Preferences.Singleton.CameraHeight;
		if (cameraDistance > maxSnapDistance)
			effectiveHeight += cameraDistance - maxSnapDistance;

		horizontalCameraPivot.Position = new Vector3(0, effectiveHeight, 0);
		springArm.SpringLength = cameraDistance;
	}

	private void RotateCameraHorizontal(float radians, bool snapping = false)
	{
		var newValue = horizontalCameraPivot.Rotation.Y + radians;
		if (newValue > Math.PI * 2)
			newValue -= (float)Math.PI * 2;
		if (newValue < -Math.PI * 2)
			newValue += (float)Math.PI * 2;

		if (newValue > Math.PI)
			newValue = -2 * (float)Math.PI + newValue;
		if (newValue < -Math.PI)
			newValue = 2 * (float)Math.PI - newValue;


		if (snapping && Math.Abs(newValue) <= 0.05f)
			newValue = 0;

		horizontalCameraPivot.Rotation = new Vector3(0, newValue, 0);
	}

	public void StopAutorun()
	{
		autorunEnabled = false;
	}

	public void ResetJumpCount()
	{
		jumpCount = 0;
	}
}
