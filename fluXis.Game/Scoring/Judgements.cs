using osu.Framework.Graphics;

namespace fluXis.Game.Scoring;

public enum Judgements
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
        new(Judgements.Flawless, "#00C3FF", 18, 1f, 0.5f),
        new(Judgements.Perfect, "#22FFB5", 40, .98f, 0.05f),
        new(Judgements.Great, "#4BFF3B", 75, .65f, 0.025f),
        new(Judgements.Alright, "#FFF12B", 100, .25f, 0),
        new(Judgements.Okay, "#F7AD40", 140, .1f, -1f),
        new(Judgements.Miss, "#FF5555", 9999, 0f, -2) // 9999 is a placeholder for infinity
    };

    public Judgements Key { get; }
    public int Timing { get; }
    public float Accuracy { get; }
    public float Health { get; }
    public Colour4 Color { get; }

    public HitWindow(Judgements key, string color, int timing, float accuracy, float health)
    {
        Key = key;
        Color = Colour4.FromHex(color);
        Timing = timing;
        Accuracy = accuracy;
        Health = health;
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

    public static HitWindow FromKey(Judgements key)
    {
        foreach (var judgement in LIST)
        {
            if (judgement.Key == key)
                return judgement;
        }

        return LIST[^1];
    }
}