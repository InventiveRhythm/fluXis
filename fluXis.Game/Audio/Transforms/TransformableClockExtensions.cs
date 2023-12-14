using osu.Framework.Graphics;
using osu.Framework.Graphics.Transforms;

namespace fluXis.Game.Audio.Transforms;

public static class TransformableClockExtensions
{
    public static TransformSequence<TransformableClock> RateTo(this TransformableClock clock, double newRate, int duration = 400, Easing easing = Easing.OutQuint)
        => clock.TransformTo(clock.PopulateTransform(new RateTransform(), newRate, duration, easing));

    public static TransformSequence<TransformableClock> RateTo(this TransformSequence<TransformableClock> t, double newRate, int duration = 400, Easing easing = Easing.OutQuint)
        => t.Append(o => o.RateTo(o.Rate, duration, easing));
}
