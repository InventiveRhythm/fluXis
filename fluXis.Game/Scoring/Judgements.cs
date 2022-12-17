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

    public class Judgement
    {
        public static readonly Judgement[] LIST =
        {
            new Judgement(Judgements.Flawless, "#00C3FF", 18, 1f),
            new Judgement(Judgements.Perfect, "#22FFB5", 40, .98f),
            new Judgement(Judgements.Great, "#4BFF3B", 75, .65f),
            new Judgement(Judgements.Alright, "#FFF12B", 100, .25f),
            new Judgement(Judgements.Okay, "#F7AD40", 140, .1f),
            new Judgement(Judgements.Miss, "#FF5555", 9999, 0f) // 9999 is a placeholder for infinity
        };

        public Judgements Key { get; }
        public int Timing { get; }
        public float Accuracy { get; }
        public Colour4 Color { get; }

        public Judgement(Judgements key, string color, int timing, float accuracy)
        {
            Key = key;
            Color = Colour4.FromHex(color);
            Timing = timing;
            Accuracy = accuracy;
        }

        public static Judgement FromTiming(float timing)
        {
            foreach (var judgement in LIST)
            {
                if (timing <= judgement.Timing)
                    return judgement;
            }

            return LIST[^1];
        }

        public static Judgement FromKey(Judgements key)
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
