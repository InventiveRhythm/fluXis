using osu.Framework.Graphics;

namespace fluXis.Game.Scoring
{
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
            new HitWindow(Judgements.Flawless, "#00C3FF", 18, 1f),
            new HitWindow(Judgements.Perfect, "#22FFB5", 40, .98f),
            new HitWindow(Judgements.Great, "#4BFF3B", 75, .65f),
            new HitWindow(Judgements.Alright, "#FFF12B", 100, .25f),
            new HitWindow(Judgements.Okay, "#F7AD40", 140, .1f),
            new HitWindow(Judgements.Miss, "#FF5555", 9999, 0f) // 9999 is a placeholder for infinity
        };

        public Judgements Key { get; }
        public int Timing { get; }
        public float Accuracy { get; }
        public Colour4 Color { get; }

        public HitWindow(Judgements key, string color, int timing, float accuracy)
        {
            Key = key;
            Color = Colour4.FromHex(color);
            Timing = timing;
            Accuracy = accuracy;
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
}
