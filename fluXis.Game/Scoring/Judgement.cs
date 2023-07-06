namespace fluXis.Game.Scoring;

public enum Judgement
{
    Flawless,
    Perfect,
    Great,
    Alright,
    Okay,
    Miss
}

public class HitWindow
{
    public static readonly HitWindow[] LIST =
    {
        new(Judgement.Flawless, 18, 1f, .5f, .5f),
        new(Judgement.Perfect, 40, .98f, .1f, .05f),
        new(Judgement.Great, 75, .65f, .05f, .025f),
        new(Judgement.Alright, 100, .25f, .02f, 0),
        new(Judgement.Okay, 140, .1f, -3, -1f),
        new(Judgement.Miss, 9999, 0f, -5, -2) // 9999 is a placeholder for infinity
    };

    public Judgement Key { get; }
    public int Timing { get; }
    public float Accuracy { get; }
    public float Health { get; }
    public float DrainRate { get; }

    public HitWindow(Judgement key, int timing, float accuracy, float health, float drainRate)
    {
        Key = key;
        Timing = timing;
        Accuracy = accuracy;
        Health = health;
        DrainRate = drainRate;
    }

    public static HitWindow FromTiming(float timing)
    {
        foreach (var judgement in LIST)
        {
            if (timing <= judgement.Timing)
                return judgement;
        }

        return LIST[^1];
    }

    public static HitWindow FromKey(Judgement key)
    {
        foreach (var judgement in LIST)
        {
            if (judgement.Key == key)
                return judgement;
        }

        return LIST[^1];
    }
}
