using Godot;

public partial class Player : CharacterBody3D
{
    [ExportCategory("PhysicsValues")]
    [Export]private float acceleration = 200;
    [Export]private float speed = 200;

    private Vector2 inputVector;

    public override void _Ready()
    {
        inputVector = Vector2.Zero;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        inputVector = Input.GetVector("left", "right", "forward", "back");
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;

        velocity.X = inputVector.X;
        velocity.Z = inputVector.Y;
        velocity = velocity.Normalized() * speed * (float)delta;

        Velocity = Velocity.MoveToward(velocity, acceleration);

        MoveAndSlide();
    }
}
