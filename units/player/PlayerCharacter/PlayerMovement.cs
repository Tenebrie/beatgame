using BeatGame.scripts.preferences;
using Godot;
using System;

namespace Project;

public partial class PlayerMovement : ComposableScript
{
	bool softCameraPreMoving = false;
	bool softCameraMoving = false;
	Vector2 softCameraMoveStart;
	bool hardCameraPreMoving = false;
	bool hardCameraMoving = false;
	Vector2 hardCameraMoveStart;

	public Vector2 InputVector = new();
	public float RotationInput;

	private Node3D horizontalCameraPivot;
	private Node3D verticalCameraPivot;
	private BetterSpringArm3D springArm;

	bool autorunEnabled = false;
	float cameraDistance = 1.5f;

	float jumpCount = 0;
	float jumpingFor = 0; // seconds

	new readonly PlayerController Parent;

	public PlayerMovement(BaseUnit parent) : base(parent)
	{
		Parent = parent as PlayerController;
	}

	public override void _Ready()
	{
		horizontalCameraPivot = Parent.GetNode<Node3D>("HCameraPivot");
		verticalCameraPivot = Parent.GetNode<Node3D>("HCameraPivot/VCameraPivot");
		springArm = Parent.GetComponent<BetterSpringArm3D>();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("SoftCameraMove".ToStringName()))
		{
			softCameraPreMoving = true;
			softCameraMoveStart = GetWindow().GetMousePosition();
		}
		if (@event.IsActionReleased("SoftCameraMove".ToStringName()))
		{
			softCameraPreMoving = false;
			softCameraMoving = false;
			if (!hardCameraMoving)
				Input.MouseMode = Input.MouseModeEnum.Visible;
		}

		if (@event.IsActionPressed("HardCameraMove".ToStringName()))
		{
			hardCameraPreMoving = true;
			hardCameraMoveStart = GetWindow().GetMousePosition();
		}
		if (@event.IsActionReleased("HardCameraMove".ToStringName()))
		{
			hardCameraPreMoving = false;
			hardCameraMoving = false;
			if (!softCameraMoving)
				Input.MouseMode = Input.MouseModeEnum.Visible;
		}

		if (@event.IsActionPressed("ZoomIn".ToStringName()))
		{
			cameraDistance = Math.Max(0, cameraDistance - 0.15f);
		}
		if (@event.IsActionPressed("ZoomOut".ToStringName()))
		{
			cameraDistance = Math.Min(4, cameraDistance + 0.15f);
		}

		if (@event.IsActionPressed("Jump".ToStringName()) && jumpCount < 2)
		{
			if (jumpingFor < 0.1f)
			{
				jumpCount = 1;
				Parent.Velocity = new Vector3(Velocity.X, 4f, Velocity.Z);
			}
			else
			{
				jumpCount = 2;
				Parent.Velocity = new Vector3(Velocity.X, 5f, Velocity.Z);
				var effect = Lib.LoadScene(Lib.Effect.DoubleJump).Instantiate<SimpleParticleEffect>();
				Parent.GetTree().CurrentScene.AddChild(effect);
				effect.SetLifetime(2);
				effect.GlobalPosition = Parent.GlobalCastAimPosition;
				effect.Emitter.DrawPass1.SurfaceSetMaterial(0, Parent.GetComponent<Visuals>().GetMaterial());
			}
		}
	}

	public override void _Process(double delta)
	{
		ProcessJumping(delta);
		ProcessMovement(delta);
		ProcessCamera(delta);
	}

	private void ProcessJumping(double delta)
	{
		if (Parent.Grounded)
		{
			jumpingFor = 0;
			return;
		}

		jumpingFor += (float)delta;
	}

	private void ProcessMovement(double delta)
	{
		var movementSpeed = 2.5f * Parent.Buffs.State.MoveSpeedPercentage;

		float movementForward = 0;
		if (Input.IsActionPressed("MoveForward".ToStringName()))
		{
			movementForward += 1;
		}
		if (Input.IsActionPressed("MoveBackward".ToStringName()))
		{
			movementForward -= 1;
		}
		if ((softCameraPreMoving || softCameraMoving) && (hardCameraPreMoving || hardCameraMoving))
		{
			movementForward += 1;
		}

		float movementRight = 0;
		if (Input.IsActionPressed("MoveRight".ToStringName()))
		{
			movementRight += 1;
		}
		if (Input.IsActionPressed("MoveLeft".ToStringName()))
		{
			movementRight -= 1;
		}
		if (Input.IsActionPressed("TurnLeft".ToStringName()) && Input.IsActionPressed("HardCameraMove".ToStringName()))
		{
			movementRight -= 1;
		}
		if (Input.IsActionPressed("TurnRight".ToStringName()) && Input.IsActionPressed("HardCameraMove".ToStringName()))
		{
			movementRight += 1;
		}

		if (Input.IsActionJustPressed("MoveForward".ToStringName()) || Input.IsActionJustPressed("MoveBackward".ToStringName()))
			autorunEnabled = false;
		if ((Input.IsActionJustPressed("SoftCameraMove".ToStringName()) && hardCameraMoving) || (Input.IsActionJustPressed("HardCameraMove".ToStringName()) && softCameraMoving))
			autorunEnabled = false;
		if (Input.IsActionJustPressed("Autorun".ToStringName()))
			autorunEnabled = !autorunEnabled;

		if (autorunEnabled)
			movementForward = 1;

		InputVector = new Vector2(movementRight, movementForward).Normalized() * movementSpeed;
		var forwardVector = -GlobalTransform.Basis.Z;
		var rightVector = GlobalTransform.Basis.X;

		var velocity = Parent.Velocity;
		Parent.Velocity = new Vector3(
			x: forwardVector.X * InputVector.Y + rightVector.X * InputVector.X,
			y: 0.001f,
			z: forwardVector.Z * InputVector.Y + rightVector.Z * InputVector.X
		);

		Parent.MoveAndSlide();

		Parent.Velocity = velocity;

		var rotationSpeed = 2; // radians per second
		RotationInput = 0;
		if (Input.IsActionPressed("TurnLeft".ToStringName()) && !hardCameraPreMoving && !hardCameraMoving)
			RotationInput += 1;
		if (Input.IsActionPressed("TurnRight".ToStringName()) && !hardCameraPreMoving && !hardCameraMoving)
			RotationInput -= 1;

		if (Math.Abs(RotationInput) > 0)
		{
			float rotation = RotationInput * rotationSpeed * (float)delta;
			Parent.Rotate(Vector3.Up, rotation);
			if (Input.IsActionPressed("SoftCameraMove".ToStringName()))
				RotateCameraHorizontal(-rotation);
		}

		if (!softCameraPreMoving && !softCameraMoving && !hardCameraPreMoving && !hardCameraMoving && InputVector.Length() > 0)
		{
			var unrotationSpeed = 3f; // radians per second

			var rotation = -Math.Sign(horizontalCameraPivot.Rotation.Y) * unrotationSpeed * (float)delta;
			if (Math.Abs(horizontalCameraPivot.Rotation.Y) <= 0.01f)
				rotation = -horizontalCameraPivot.Rotation.Y;
			RotateCameraHorizontal(rotation, true);
		}
	}

	private void ProcessCamera(double delta)
	{
		if (hardCameraPreMoving)
		{
			var mousePos = GetWindow().GetMousePosition();
			var mouseDelta = mousePos - hardCameraMoveStart;
			if (mouseDelta.Length() > 10 || softCameraMoving)
			{
				hardCameraPreMoving = false;
				hardCameraMoving = true;
				Input.MouseMode = Input.MouseModeEnum.Hidden;
				Parent.Rotate(Vector3.Up, horizontalCameraPivot.Rotation.Y);
				RotateCameraHorizontal(-horizontalCameraPivot.Rotation.Y);
				SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CameraMovingStarted);
			}
		}
		else if (hardCameraMoving)
		{
			var mousePos = GetWindow().GetMousePosition();
			var mouseDelta = mousePos - hardCameraMoveStart;
			Input.WarpMouse(hardCameraMoveStart * GetTree().Root.ContentScaleFactor);
			Parent.Rotate(Vector3.Up, -mouseDelta.X / 500);
			var rotation = (float)Math.Min(Math.PI / 2, Math.Max(-Math.PI / 2 + 0.01f, verticalCameraPivot.Rotation.X - mouseDelta.Y / 500));
			verticalCameraPivot.Rotation = new Vector3(rotation, 0, 0);
		}
		else if (softCameraPreMoving)
		{
			var mousePos = GetWindow().GetMousePosition();
			var mouseDelta = mousePos - softCameraMoveStart;
			if (mouseDelta.Length() > 10 || hardCameraMoving)
			{
				softCameraPreMoving = false;
				softCameraMoving = true;
				Input.MouseMode = Input.MouseModeEnum.Hidden;
				SignalBus.Singleton.EmitSignal(SignalBus.SignalName.CameraMovingStarted);
			}
		}
		else if (softCameraMoving)
		{
			var mousePos = GetWindow().GetMousePosition();
			var mouseDelta = mousePos - softCameraMoveStart;
			Input.WarpMouse(softCameraMoveStart * GetTree().Root.ContentScaleFactor);
			RotateCameraHorizontal(-mouseDelta.X / 500);
			var verticalRotation = (float)Math.Min(Math.PI / 2, Math.Max(-Math.PI / 2 + 0.01f, verticalCameraPivot.Rotation.X - mouseDelta.Y / 500));
			verticalCameraPivot.Rotation = new Vector3(verticalRotation, 0, 0);
		}

		var effectiveHeight = Preferences.Singleton.CameraHeight + Parent.CastAimPosition.Y;
		const float minSnapDistance = 1.5f;
		const float maxSnapDistance = 2.5f;
		switch (cameraDistance)
		{
			case < minSnapDistance:
				effectiveHeight -= (1 - (cameraDistance / minSnapDistance)) * Preferences.Singleton.CameraHeight;
				break;
			case > maxSnapDistance:
				effectiveHeight += cameraDistance - maxSnapDistance;
				break;
		}

		var currentHeight = horizontalCameraPivot.Position.Y;
		horizontalCameraPivot.Position = new Vector3(0, currentHeight + (effectiveHeight - currentHeight) * 5 * (float)delta, 0);
		springArm.SpringLength = cameraDistance;

		Parent.Visible = horizontalCameraPivot.Position.Y - Parent.CastAimPosition.Y > 0.01;
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

	public bool IsMovingCamera()
	{
		return softCameraMoving || hardCameraMoving;
	}
}
