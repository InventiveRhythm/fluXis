using System;
using System.Linq;
using fluXis.Game.Map;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Transforms;
using osu.Framework.Timing;
using osu.Framework.Utils;

namespace fluXis.Game.Screens.Edit;

public partial class EditorClock : CompositeComponent, IFrameBasedClock, IAdjustableClock, ISourceChangeableClock
{
    public IBindable<Track> Track => track;
    public float TrackLength => (float)(track.Value?.Length ?? 10000);

    public MapInfo MapInfo { get; set; }

    public double ElapsedFrameTime => underlying.ElapsedFrameTime;
    public double FramesPerSecond => underlying.FramesPerSecond;
    public FrameTimeInfo TimeInfo => underlying.TimeInfo;
    public double CurrentTime => underlying.CurrentTime;
    public double CurrentTimeAccurate => Transforms.OfType<TimeTransform>().FirstOrDefault()?.EndValue ?? CurrentTime;
    public IClock Source => underlying.Source;
    public bool IsRunning => underlying.IsRunning;
    double IClock.Rate => underlying.Rate;

    double IAdjustableClock.Rate
    {
        get => underlying.Rate;
        set => underlying.Rate = value;
    }

    public double Rate
    {
        get => rate.Value;
        set => rate.Value = value;
    }

    private readonly FramedMapClock underlying;
    private readonly Bindable<Track> track = new();
    private readonly Bindable<double> rate = new(1);

    private bool playbackFinished;

    public EditorClock(MapInfo mapInfo)
    {
        MapInfo = mapInfo;
        underlying = new FramedMapClock { IsCoupled = false };
        AddInternal(underlying);
    }

    public bool SeekSnapped(float position)
    {
        return Seek(Snap(position));
    }

    public void SeekSmoothly(double time)
    {
        if (IsRunning)
            Seek(time);
        else
            transformTimeTo(time, 300, Easing.OutQuint);
    }

    public double Snap(double position)
    {
        var point = MapInfo.GetTimingPoint((float)position);
        float snapLength = point.Signature * point.MsPerBeat / (4 * 4);
        position -= point.Time;

        int closest = (int)Math.Round(position / snapLength);
        position = point.Time + closest * snapLength;

        var nextPoint = MapInfo.TimingPoints.FirstOrDefault(x => x.Time > point.Time);
        if (position > nextPoint?.Time)
            position = nextPoint.Time;

        return position;
    }

    public void Reset()
    {
        ClearTransforms();
        underlying.Reset();
    }

    public void Start()
    {
        ClearTransforms();

        if (playbackFinished)
            underlying.Seek(0);

        underlying.Start();
    }

    public void Stop()
    {
        underlying.Stop();
    }

    public bool Seek(double position)
    {
        ClearTransforms();
        position = Math.Clamp(position, 0, TrackLength);
        return underlying.Seek(position);
    }

    public void SeekBackward(double amount) => seek(-1, amount + (IsRunning ? 1.5 : 0));
    public void SeekForward(double amount) => seek(1, amount);

    private void seek(int direction, double amount)
    {
        if (amount <= 0) return;

        double time = CurrentTimeAccurate;
        var tp = MapInfo.GetTimingPoint((float)time);

        if (direction < 0 && tp.Time == time)
            tp = MapInfo.GetTimingPoint((float)(time - 1));

        double sAmount = tp.MsPerBeat / 4 * amount;
        double sTime = time + sAmount * direction;

        if (IsRunning || MapInfo.TimingPoints.Count == 0)
        {
            SeekSmoothly(sTime);
            return;
        }

        sTime -= tp.Time;

        int closest;
        if (direction > 0) closest = (int)Math.Floor(sTime / sAmount);
        else closest = (int)Math.Ceiling(sTime / sAmount);

        sTime = tp.Time + closest * sAmount;

        var nextTimingPoint = MapInfo.TimingPoints.FirstOrDefault(t => t.Time > tp.Time);
        if (sTime > nextTimingPoint?.Time)
            sTime = nextTimingPoint.Time;

        if (Precision.AlmostEquals(time, sTime, 0.5f))
        {
            closest += direction > 0 ? 1 : -1;
            sTime = tp.Time + closest * sAmount;
        }

        if (sTime < tp.Time && !ReferenceEquals(tp, MapInfo.TimingPoints.First()))
            sTime = tp.Time;

        SeekSmoothly(sTime);
    }

    protected override void Update()
    {
        base.Update();

        playbackFinished = CurrentTime >= TrackLength;

        if (playbackFinished)
        {
            if (IsRunning) underlying.Stop();
            if (CurrentTime > TrackLength) underlying.Seek(TrackLength);
        }
    }

    public void ResetSpeedAdjustments() => underlying.ResetSpeedAdjustments();

    public void ProcessFrame() { }

    public void ChangeSource(IClock source)
    {
        track.Value = source as Track;
        Track.Value.AddAdjustment(AdjustableProperty.Frequency, rate);
        underlying.ChangeSource(source);
    }

    private void transformTimeTo(double time, double duration, Easing easing) => this.TransformTo(this.PopulateTransform(new TimeTransform(), Math.Clamp(time, 0, TrackLength), duration, easing));

    private double currentTime
    {
        get => underlying.CurrentTime;
        set => underlying.Seek(value);
    }

    private class TimeTransform : Transform<double, EditorClock>
    {
        public override string TargetMember => nameof(currentTime);

        protected override void Apply(EditorClock clock, double time) => clock.currentTime = valueAt(time);

        private double valueAt(double time)
        {
            if (time < StartTime) return StartValue;
            if (time >= EndTime) return EndValue;

            return Interpolation.ValueAt(time, StartValue, EndValue, StartTime, EndTime, Easing);
        }

        protected override void ReadIntoStartValue(EditorClock clock) => StartValue = clock.currentTime;
    }
}
