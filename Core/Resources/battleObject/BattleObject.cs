using Godot;
using System.Threading.Tasks;

public enum BattleObjectKind
{
    Player,
    Enemy
}

public partial class BattleObject : Node3D
{
    [Signal]
    public delegate void TurnStartedEventHandler();

    [Signal]
    public delegate void TurnEndedEventHandler();

    [ExportCategory("Nodes")]
    [Export]protected BattleFieldManager battleManager;

    protected BattleObjectKind kind;
    private Camera3D camera;
    private Transform3D defaultCameraPosition;
    private Marker3D cameraMarker;
    protected Sprite3D objectSprite;
    protected bool readyToFight;
    protected ActiveBattleTimer activeBattleTimer;
    protected int battlePrepSpeed;
    protected int battleMoveSpeed;
    protected float movedThisTurn;
    protected Vector2I mapLoc;

    public BattleFieldManager BattleManager {set => battleManager = value;}

    public virtual Vector2I MapLoc
    {
        set
        {
            mapLoc = value;

            Position = new(mapLoc.X * SizeValues.Inst.GridSizeM, Position.Y, mapLoc.Y * SizeValues.Inst.GridSizeM);
        }
    }

    protected bool ReadyToFight
    {
        set
        {
            if(readyToFight == value)
            {
                return;
            }

            readyToFight = value;

            if (readyToFight)
            {
                _ = StartTurn();
            }
            else
            {
                _ = EndTurn();
            }
        }
    }

    public BattleObjectKind Kind {get => kind;}

    public BattleObject()
    {
        activeBattleTimer = new();
        readyToFight = false;
        movedThisTurn = 0;
    }

    public override void _Ready()
    {
        camera = GetViewport().GetCamera3D();
        defaultCameraPosition = camera.GlobalTransform;
        cameraMarker = GetNodeOrNull<Marker3D>("%CameraMarker");
        objectSprite = GetNode<Sprite3D>("%ObjectSprite");
    }

    public override void _Process(double delta)
    {
        if (battleManager.TurnBeingTaken && !readyToFight)
        {
            return;
        }

        if(!readyToFight)
        {
            ReadyToFight = activeBattleTimer.incrementTimer(battlePrepSpeed, delta);
            return;
        }

        TakeTurn(delta);
    }

    protected virtual async Task StartTurn(float duration = 1)
    {
        EmitSignal(BattleObject.SignalName.TurnStarted);

        Tween tween = CreateTween();

        tween.TweenProperty(camera, "global_transform", cameraMarker.GlobalTransform, duration);

        await ToSignal(tween, Tween.SignalName.Finished);
    }

    protected virtual async Task EndTurn(float duration = 1)
    {
        EmitSignal(BattleObject.SignalName.TurnEnded);

        Tween tween = CreateTween();

        tween.TweenProperty(camera, "global_transform", defaultCameraPosition, duration);

        await ToSignal(tween, Tween.SignalName.Finished);
    }

    protected virtual void TakeTurn(double delta)
    {

    }
}
