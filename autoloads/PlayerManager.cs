using Godot;

public partial class PlayerManager : Node
{
    private int health;
    private int battleMoveSpeed;
    private int battlePrepSpeed;

    public int Health
    {
        get=>health;
    }

    public int BattleMoveSpeed
    {
        get=>battleMoveSpeed;
    }

    public int BattlePrepSpeed
    {
        get => battlePrepSpeed;
    }

    public PlayerManager(int health, int battleMoveSpeed, int battlePrepSpeed)
    {
        this.health = health;
        this.battleMoveSpeed = battleMoveSpeed;
        this.battlePrepSpeed = battlePrepSpeed;
    }
}
