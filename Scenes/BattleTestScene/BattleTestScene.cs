using System;

public partial class BattleTestScene : BattleFieldManager
{
    public BattleTestScene():base()
    {
        Random rand = new();

        partyStartLocs = rand.Next(1, 2) switch
        {
            1 => [new(0, 9), new(2, 9)],
            _ => []
        };
    }
}
