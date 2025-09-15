using Godot;

public partial class FlightController : Camera3D
{
	[Export] public float CameraSpeed = 1f;
	[Export] public float MouseSensitivity = 0.5f;

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			float rotationX = RotationDegrees.X - mouseMotion.Relative.Y * MouseSensitivity;
			rotationX = Mathf.Clamp(rotationX, -90f, 90f);

			float rotationY = RotationDegrees.Y - mouseMotion.Relative.X * MouseSensitivity;

			RotationDegrees = new Vector3(rotationX, rotationY, 0f);
		}
		if (@event.IsActionPressed("escape_mouse"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
	}

	public override void _Process(double delta)
	{
		Vector3 direction = Vector3.Zero;

		if (Input.IsActionPressed("move_forward"))
		{
			direction -= Transform.Basis.Z;
		}
		if (Input.IsActionPressed("move_back"))
		{
			direction += Transform.Basis.Z;
		}
		if (Input.IsActionPressed("move_right"))
		{
			direction += Transform.Basis.X;
		}
		if (Input.IsActionPressed("move_left"))
		{
			direction -= Transform.Basis.X;
		}
		if (Input.IsActionPressed("move_up"))
		{
			direction += Transform.Basis.Y;
		}
		if (Input.IsActionPressed("move_down"))
		{
			direction -= Transform.Basis.Y;
		}

		if (direction != Vector3.Zero)
		{
			direction = direction.Normalized();
			Position += direction * CameraSpeed * (float)delta;
		}
	}

}
