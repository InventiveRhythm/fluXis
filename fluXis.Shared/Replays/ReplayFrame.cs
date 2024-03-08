namespace fluXis.Shared.Replays;

public class ReplayFrame
{
    public float Time { get; set; }
    public List<int> Actions { get; init; }

    public ReplayFrame(float time, params int[] actions)
    {
        Time = time;
        Actions = new List<int>(actions);
    }
}
