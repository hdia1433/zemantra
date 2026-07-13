public partial class BattleTestPlayer : BattlePlayer
{
    public BattleTestPlayer():base()
    {
        actions.Add(BattlePlayerAction.Move);
        actions.Add(BattlePlayerAction.EndTurn);

        actionMenuId = "TestPlayer";
        manager = PartyManager.Inst.Managers[(int)PartyMember.Test];
    }

    public override void _Ready()
    {
        base._Ready();
    }
}
