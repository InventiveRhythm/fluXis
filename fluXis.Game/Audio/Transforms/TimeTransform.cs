using osu.Framework.Graphics.Transforms;
using osu.Framework.Utils;

namespace fluXis.Game.Audio.Transforms;

public class TimeTransform : Transform<double, TransformableClock>
{
    public override string TargetMember => "CurrentTime";

    protected override void Apply(TransformableClock clock, double time) => clock.Seek(valueAt(time));

    private double valueAt(double time)
    {
        if (time < StartTime) return StartValue;
        if (time >= EndTime) return EndValue;

        return Interpolation.ValueAt(time, StartValue, EndValue, StartTime, EndTime, Easing);
    }

    protected override void ReadIntoStartValue(TransformableClock clock) => StartValue = clock.CurrentTime;
}
