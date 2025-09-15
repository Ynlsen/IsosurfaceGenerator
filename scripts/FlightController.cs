using Godot;

public partial class FlightController : Camera3D
{
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
			Position += direction * (float) delta;
		}
	}

}
