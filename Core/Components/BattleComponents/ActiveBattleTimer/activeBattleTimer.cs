public class ActiveBattleTimer
{
    private float progress;
    private int totalTime;

    public ActiveBattleTimer()
    {
        progress = 0;
        totalTime = 10;
    }

    public float Progress {get => progress;}

    public bool incrementTimer(int speed, double delta)
    {
        progress += speed * (float)delta;

        if (progress >= totalTime)
        {
            progress = 0;
            return true;
        }

        return false;
    }
}
