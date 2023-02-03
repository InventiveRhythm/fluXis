namespace fluXis.Game.Utils;

public class Range
{
    public int Min { get; set; }
    public int Max { get; set; }

    public bool Same => Min == Max;

    public Range()
    {
    }
}
