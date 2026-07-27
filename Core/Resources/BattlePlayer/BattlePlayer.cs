using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

enum BattlePlayerState
{
    Waiting,
    SelectingAction,
    SelectingMove
}

///<summary>
///The generic player object to be used in battles
///</summary>
public partial class BattlePlayer:BattleObject
{
    protected List<BattlePlayerAction> actions;
    protected string actionMenuId;
    private BattlePlayerState playerState;
    protected PlayerActionsMenu actionMenu;
    protected PlayerManager manager;

    private MoveSelectableMesh moveSelectableMesh;
    private MoveSelector moveSelector;

    public List<BattlePlayerAction> Actions
    {get => actions;}

    ///<summary>
    ///Changes the state the player is in and executes code so that the player's members reflect its new state
    ///</summary>
    private BattlePlayerState PlayerState
    {
        set
        {
            if(playerState == value)
            {
                return;
            }

            switch(playerState)
            {
                case BattlePlayerState.SelectingAction:
                    SceneManager.scene.HideLoadedHud();
                    break;
                case BattlePlayerState.SelectingMove:
                    Tween tween = CreateTween();
                    tween.TweenProperty(objectSprite, "modulate:a", 1, 1);
                    moveSelectableMesh.clearIndecis();
                    break;
            }

            playerState = value;

            switch(playerState)
            {
                case BattlePlayerState.SelectingAction:
                    SceneManager.scene.SwitchToLoadedHud(actionMenuId, false);
                    Tween tween = CreateTween();
                    tween.TweenProperty(camera, "global_transform", cameraMarker.GlobalTransform, 1);
                    break;

            }
        }
    }

    ///<summary>
    ///Changes the current Map Location variable, updates the map to reflect the change, and updates the position of the player to reflect the change as well.
    ///</summary>
    public override Vector2I MapLoc
    {
        set
        {
            if(battleManager != null)
            {
                battleManager.Map[mapLoc.X, mapLoc.Y] = null;
            }

            mapLoc = value;
            moveSelector.BoardLocation = value;

            if(battleManager != null)
            {
                battleManager.Map[mapLoc.X, mapLoc.Y] = this;
            }

            Position = new(mapLoc.X * SizeValues.Inst.GridSizeM, Position.Y, mapLoc.Y * SizeValues.Inst.GridSizeM);
        }
    }

    public MultiMeshInstance3D MoveSelectableMesh {get => moveSelectableMesh;}

    public BattlePlayer():base()
    {
        kind = BattleObjectKind.Player;

        actions = new();
        PlayerState = BattlePlayerState.Waiting;
    }

    public override void _Ready()
    {
        base._Ready();

        actionMenu = (PlayerActionsMenu)GD.Load<PackedScene>("res://UI/PlayerActions/playerActionsMenu.tscn").Instantiate();
        actionMenu.Player = this;

        SceneManager.scene.AddLoadedHud(actionMenu, actionMenuId);

        battleMoveSpeed = manager.BattleMoveSpeed;
        battlePrepSpeed = manager.BattlePrepSpeed;

        moveSelectableMesh = GetNode<MoveSelectableMesh>("%MoveSelectableMesh");

        moveSelector = GetNode<MoveSelector>("%MoveSelector");
    }

    protected override async Task StartTurn(float duration = 1)
    {
        await base.StartTurn(duration);

        PlayerState = BattlePlayerState.SelectingAction;
        actionMenu.EnableAllActions();
    }

    protected override async Task EndTurn(float duration = 1)
    {
        PlayerState = BattlePlayerState.Waiting;

        await base.EndTurn(duration);
    }

    protected override void TakeTurn(double delta)
    {
        switch (playerState)
        {
            case BattlePlayerState.Waiting:
                break;
            case BattlePlayerState.SelectingAction:
                break;
            case BattlePlayerState.SelectingMove:
                SelectingMove();
                break;
        }
    }

    ///<summary>
    ///Runs all the setup for the player's movement state
    ///</summary>
    public void StartMoving()
    {
        PlayerState = BattlePlayerState.SelectingMove;

        int count = 0;

        for(int x = 0; x < battleManager.Map.GetLength(0); x++)
        {
            for(int y = 0; y < battleManager.Map.GetLength(1); y++)
            {
                if(CustomMath.Inst.Square(x - mapLoc.X) + CustomMath.Inst.Square(y - mapLoc.Y) > CustomMath.Inst.Square((int)(battleMoveSpeed - movedThisTurn)))
                {
                    continue;
                }
                moveSelectableMesh.AddIndex(new(x, y), count++);
            }
        }

        MultiMesh multiMesh = moveSelectableMesh.Multimesh;
        multiMesh.InstanceCount = count;

        for(int x = 0; x < battleManager.Map.GetLength(0); x++)
        {
            for(int y = 0; y < battleManager.Map.GetLength(1); y++)
            {
                if(CustomMath.Inst.Square(x - mapLoc.X) + CustomMath.Inst.Square(y - mapLoc.Y) > CustomMath.Inst.Square((int)(battleMoveSpeed - movedThisTurn)))
                {
                    continue;
                }


                Vector2I loc = new(x, y);
                int i = moveSelectableMesh.LocToIndex(loc);

                multiMesh.SetInstanceTransform(i, new(Basis.Identity, new(x * SizeValues.Inst.GridSizeM + SizeValues.Inst.GridSizeM / 2f, 0.001f, y * SizeValues.Inst.GridSizeM + SizeValues.Inst.GridSizeM / 2f)));
                multiMesh.SetInstanceColor(i, loc == moveSelector.BoardLocation ? Colors.Yellow:
                        battleManager.Map[x, y] is null || loc == mapLoc ? Colors.Blue: Colors.Red);
            }
        }

        moveSelector.InFocus = true;

        Tween tween = CreateTween();
        tween.TweenProperty(objectSprite, "modulate:a", .33, 1);
    }

    ///<summary>
    ///Holds the logic for controlling the selector for the square the player will move to.
    ///</summary>
    private void SelectingMove()
    {

        Vector2I movement = Vector2I.Zero;

        if(Input.IsActionJustReleased("forward"))
        {
            movement = new(0, -1);
        }
        else if(Input.IsActionJustReleased("back"))
        {
            movement = new(0 ,1);
        }
        else if(Input.IsActionJustReleased("left"))
        {
            movement = new(-1, 0);
        }
        else if(Input.IsActionJustReleased("right"))
        {
            movement = new(1, 0);
        }

        if(Input.IsActionJustReleased("enter"))
        {
            movedThisTurn += Mathf.Sqrt(CustomMath.Inst.Square(moveSelector.BoardLocation.X - mapLoc.X) + CustomMath.Inst.Square(moveSelector.BoardLocation.Y - mapLoc.Y));

            MapLoc = moveSelector.BoardLocation;
            MoveSelectableMesh.Multimesh.InstanceCount = 0;

            if((int)(battleMoveSpeed - movedThisTurn) == 0)
            {
                PlayerState = BattlePlayerState.SelectingAction;
                actionMenu.DisableAction(BattlePlayerAction.Move);
            }
            else
            {
                moveSelectableMesh.clearIndecis();
                StartMoving();
            }
        }

        if(Input.IsActionJustReleased("esc"))
        {
            PlayerState = BattlePlayerState.SelectingAction;
        }

        Vector2I newLoc = moveSelector.BoardLocation + movement;

        if(newLoc.X < 0 || newLoc.Y < 0 || newLoc.X > battleManager.Map.GetLength(0) - 1 || newLoc.Y >  battleManager.Map.GetLength(1) - 1)
        {
            return;
        }
        else if(movement == Vector2I.Zero)
        {
            return;
        }
        else if(CustomMath.Inst.Square(newLoc.X - mapLoc.X) + CustomMath.Inst.Square(newLoc.Y - mapLoc.Y) > CustomMath.Inst.Square((int)(battleMoveSpeed - movedThisTurn)))
        {
            return;
        }
        else if(battleManager.Map[newLoc.X, newLoc.Y] != null && newLoc != mapLoc)
        {
            return;
        }

        MultiMesh multiMesh = moveSelectableMesh.Multimesh;

        multiMesh.SetInstanceColor(moveSelectableMesh.LocToIndex(moveSelector.BoardLocation), Colors.Blue);
        multiMesh.SetInstanceColor(moveSelectableMesh.LocToIndex(newLoc), Colors.Yellow);
        moveSelector.BoardLocation += movement;
    }

    ///<summary>
    ///Runs the code necessary to end the current turn for the player
    ///</summary>
    public void EndCurrentTurn()
    {
        ReadyToFight = false;
        movedThisTurn = 0;
    }
}
