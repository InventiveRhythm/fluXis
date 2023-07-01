using osu.Framework.Graphics;

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
        new(Judgement.Flawless, "#00C3FF", 18, 1f, .5f, .5f),
        new(Judgement.Perfect, "#22FFB5", 40, .98f, .1f, .05f),
        new(Judgement.Great, "#4BFF3B", 75, .65f, .05f, .025f),
        new(Judgement.Alright, "#FFF12B", 100, .25f, .02f, 0),
        new(Judgement.Okay, "#F7AD40", 140, .1f, -3, -1f),
        new(Judgement.Miss, "#FF5555", 9999, 0f, -5, -2) // 9999 is a placeholder for infinity
    };

    public Judgement Key { get; }
    public int Timing { get; }
    public float Accuracy { get; }
    public float Health { get; }
    public float DrainRate { get; }
    public Colour4 Color { get; }

    public HitWindow(Judgement key, string color, int timing, float accuracy, float health, float drainRate)
    {
        Key = key;
        Color = Colour4.FromHex(color);
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
