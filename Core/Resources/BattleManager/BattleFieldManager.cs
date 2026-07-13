using Godot;

public partial class BattleFieldManager : Node3D
{
    private Node3D battleObjectsParent;
    private BattleObject[,] map;
    protected Vector2I[] partyStartLocs;

    public bool TurnBeingTaken
    { get; private set; }

    public BattleObject[,] Map 
    {
        get => map;
    }

    public BattleFieldManager()
    {
        map = new BattleObject[10, 10];
    }

    public override void _Ready()
    {
        TurnBeingTaken = false;

        battleObjectsParent = GetNode<Node3D>("%BattleObjects");

        for (int i = 0; i < PartyManager.Inst.Members.Count; i++)
        {
            BattlePlayer battlePlayer = PartyManager.Inst.Members[i] switch
            {
                PartyMember.Test => (BattleTestPlayer)GD.Load<PackedScene>("res://Model/Player/BattleTestPlayer/BattleTestPlayer.tscn").Instantiate(),
                _ => new()
            };

            battleObjectsParent.AddChild(battlePlayer);
            battlePlayer.Position = new(partyStartLocs[i].X * SizeValues.Inst.GridSizeM, 0, partyStartLocs[i].Y * SizeValues.Inst.GridSizeM);
            map[partyStartLocs[i].X, partyStartLocs[i].Y] = battlePlayer;
            battlePlayer.MapLoc = partyStartLocs[i];
            battlePlayer.BattleManager = this;

            battlePlayer.MoveSelectableMesh.GlobalTransform = Transform3D.Identity;
        }

        foreach (Node3D node3D in battleObjectsParent.GetChildren())
        {
            var battleObject = (BattleObject)node3D;

            battleObject.TurnStarted += () => TurnBeingTaken = true;
            battleObject.TurnEnded += () => TurnBeingTaken = false;
        }
    }
}
