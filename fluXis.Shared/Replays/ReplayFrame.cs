namespace fluXis.Shared.Replays;

public class ReplayFrame
{
    public double Time { get; set; }
    public List<int> Actions { get; init; }

    public ReplayFrame(double time, params int[] actions)
    {
        Time = time;
        Actions = new List<int>(actions);
    }
}
