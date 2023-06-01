using osu.Framework.Graphics.Transforms;
using osu.Framework.Utils;

namespace fluXis.Game.Audio.Transforms;

public class RateTransform : Transform<double, TransformableClock>
{
    public override string TargetMember => "Rate";

    protected override void Apply(TransformableClock clock, double time) => clock.Rate = valueAt(time);

    private double valueAt(double time)
    {
        if (time < StartTime) return StartValue;
        if (time >= EndTime) return EndValue;

        return Interpolation.ValueAt(time, StartValue, EndValue, StartTime, EndTime, Easing);
    }

    protected override void ReadIntoStartValue(TransformableClock clock) => StartValue = clock.Rate;
}
