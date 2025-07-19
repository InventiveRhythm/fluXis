using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Transforms;
using osu.Framework.Timing;
using osu.Framework.Utils;

namespace fluXis.Audio.Transforms;

public abstract partial class TransformableClock : CompositeComponent, IAdjustableClock, IFrameBasedClock
{
    public abstract double CurrentTime { get; }
    public abstract void Reset();
    public abstract void Start();
    public abstract void Stop();
    public abstract bool Seek(double position);
    public abstract bool SeekForce(double position);
    public abstract void ResetSpeedAdjustments();
    double IAdjustableClock.Rate { get; set; }
    double IClock.Rate => Rate;
    public abstract bool IsRunning { get; }
    public abstract double ElapsedFrameTime { get; }
    public abstract double FramesPerSecond { get; }

    public double Rate
    {
        get => RateBindable.Value;
        set => RateBindable.Value = value;
    }

    public BindableNumber<double> RateBindable { get; } = new(1)
    {
        MinValue = 0f,
        MaxValue = 2f,
        Precision = .05f
    };

    public TransformSequence<TransformableClock> TimeTo(double newTime, double duration = 0, Easing easing = Easing.None)
        => this.TransformTo(this.PopulateTransform(new TimeTransform(), newTime, duration, easing));

    public double InterpolateAt(double elapsedTime, double newTime, double duration, Easing easing = Easing.None)
        => Interpolation.ValueAt(elapsedTime, CurrentTime, newTime, 0, duration, easing);

    public abstract void ProcessFrame();
}
