using System;
using System.Linq;
using fluXis.Game.Audio.Transforms;
using fluXis.Game.Map;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Timing;
using osu.Framework.Utils;

namespace fluXis.Game.Screens.Edit;

public partial class EditorClock : TransformableClock, IFrameBasedClock, ISourceChangeableClock
{
    public IBindable<Track> Track => track;
    public float TrackLength => (float)(track.Value?.Length ?? 10000);

    public event Action<Track> TrackChanged;

    public MapInfo MapInfo { get; set; }
    public BindableInt SnapDivisor { get; init; }

    public double ElapsedFrameTime => underlying.ElapsedFrameTime;
    public double FramesPerSecond => underlying.FramesPerSecond;
    public FrameTimeInfo TimeInfo => underlying.TimeInfo;
    public override double CurrentTime => underlying.CurrentTime;
    public IClock Source => underlying.Source;
    public override bool IsRunning => underlying.IsRunning;
    double IClock.Rate => underlying.Rate;

    private double currentTimeAccurate => Transforms.OfType<TimeTransform>().FirstOrDefault()?.EndValue ?? CurrentTime;

    public double BeatTime { get; private set; }

    private readonly FramedMapClock underlying;
    private readonly Bindable<Track> track = new();

    private bool playbackFinished;

    public EditorClock(MapInfo mapInfo)
    {
        MapInfo = mapInfo;
        underlying = new FramedMapClock();
        AddInternal(underlying);
    }

    public bool SeekSnapped(float position)
    {
        return Seek(snap(position));
    }

    public void SeekSmoothly(double time)
    {
        time = Math.Clamp(time, 0, TrackLength);

        if (IsRunning)
            Seek(time);
        else
            TimeTo(time, 300, Easing.OutQuint);
    }

    private double snap(double position)
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

    public override void Reset()
    {
        ClearTransforms();
        underlying.Reset();
    }

    public override void Start()
    {
        ClearTransforms();

        if (playbackFinished)
            underlying.Seek(0);

        underlying.Start();
    }

    public override void Stop()
    {
        underlying.Stop();
    }

    public override bool Seek(double position)
    {
        ClearTransforms();
        position = Math.Clamp(position, 0, TrackLength);
        return underlying.Seek(position);
    }

    public override bool SeekForce(double position) => underlying.Seek(position);

    public void SeekBackward(double amount) => seek(-1, amount + (IsRunning ? 1.5 : 0));
    public void SeekForward(double amount) => seek(1, amount);

    private void seek(int direction, double amount)
    {
        if (amount <= 0) return;

        double time = currentTimeAccurate;
        var tp = MapInfo.GetTimingPoint((float)time);

        if (direction < 0 && tp.Time == time)
            tp = MapInfo.GetTimingPoint((float)(time - 1));

        double sAmount = tp.MsPerBeat / SnapDivisor.Value * amount;
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

        var tp = MapInfo.GetTimingPoint((float)CurrentTime);
        BeatTime = tp.MsPerBeat;
    }

    public override void ResetSpeedAdjustments() => underlying.ResetSpeedAdjustments();

    public void ProcessFrame() { }

    public void ChangeSource(IClock source)
    {
        track.Value?.Dispose();
        track.Value = source as Track;
        Track.Value.AddAdjustment(AdjustableProperty.Frequency, RateBindable);
        underlying.ChangeSource(source);

        TrackChanged?.Invoke(Track.Value);
    }
}
