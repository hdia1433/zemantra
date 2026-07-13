using Godot;

public partial class MoveSelector : Node3D
{
    private Vector2I boardLocation;
    private bool inFocus;

    private Marker3D cameraMarker;
    private Camera3D camera;

    public Vector2I BoardLocation
    {
        get => boardLocation;
        set
        {
            boardLocation = value;

            GlobalPosition = new(boardLocation.X * SizeValues.Inst.GridSizeM + SizeValues.Inst.GridSizeMHalf, GlobalPosition.Y, boardLocation.Y * SizeValues.Inst.GridSizeM + SizeValues.Inst.GridSizeMHalf);

            if(inFocus)
            {
                Tween tween = CreateTween();

                tween.TweenProperty(camera, "global_transform", cameraMarker.GlobalTransform, .25);
            }
        }
    }

    public bool InFocus
    {
        get => inFocus;
        set
        {
            inFocus = value;

            if(inFocus)
            {
                Tween tween = CreateTween();

                tween.TweenProperty(camera, "global_transform", cameraMarker.GlobalTransform, 1);
            }
        }
    }

    public MoveSelector()
    {
        boardLocation = new();
        inFocus = false;
    }

    public override void _Ready()
    {
        cameraMarker = GetNode<Marker3D>("%CameraMarker");

        camera = GetViewport().GetCamera3D();
    }
}
