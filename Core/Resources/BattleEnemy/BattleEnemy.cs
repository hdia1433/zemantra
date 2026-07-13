using Godot;
using System.Threading.Tasks;

public partial class BattleEnemy : BattleObject
{
    private Timer turnTimer;
    private bool waiting;

    public BattleEnemy(): base()
    {
        kind = BattleObjectKind.Enemy;

        waiting = false;
    }

    public override void _Ready()
    {
        base._Ready();

        turnTimer = GetNode<Timer>("%TurnTimer");
        turnTimer.Timeout += () => waiting = false;
    }

    private void StartTimer()
    {
        turnTimer.Start();
        waiting = true;
    }

    protected override async Task StartTurn(float duration = 1)
    {
        waiting = true;
        await base.StartTurn(duration);
        waiting = false;

        StartTimer();
    }

    protected override void TakeTurn(double delta)
    {
        if (!waiting)
        {
            ReadyToFight = false;
        }
    }
}
