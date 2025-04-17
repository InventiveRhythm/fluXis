using System;
using System.Linq;
using fluXis.Audio;
using fluXis.Audio.Transforms;
using fluXis.Configuration;
using fluXis.Map;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Audio;
using osu.Framework.Timing;

namespace fluXis.Screens.Gameplay.Audio;

public partial class GameplayClock : TransformableClock, IFrameBasedClock, ISourceChangeableClock, IBeatSyncProvider
{

    public float Offset => useOffset ? offset.Value : 0;
    private bool useOffset { get; }

    public override double CurrentTime => underlying.CurrentTime - Offset;
    public override bool IsRunning => underlying.IsRunning;
    public override double ElapsedFrameTime => underlying.ElapsedFrameTime;
    public override double FramesPerSecond => underlying.FramesPerSecond;
    public IClock Source => underlying.Source;

    private MapInfo mapInfo { get; }
    private ITrackStore tracks { get; }

    public DrawableTrack Track { get; private set; }

    private FramedMapClock underlying { get; }
    private Bindable<float> offset;

    public event Action<double, double> OnSeek;

    public GameplayClock(ITrackStore tracks, MapInfo info, Track track, bool useOffset)
    {
        this.tracks = tracks;
        this.useOffset = useOffset;

        underlying = new FramedMapClock();
        AddInternal(underlying);

        mapInfo = info;
        ChangeSource(track);
    }

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        offset = config.GetBindable<float>(FluXisSetting.GlobalOffset);
    }

    public override void Start()
    {
        underlying.Start();
    }

    public override void Stop()
    {
        underlying.Stop();
    }

    public override void Reset()
    {
        underlying.Reset();
    }

    public override bool Seek(double position)
    {
        position = Math.Max(-2000, position);

        var current = CurrentTime;
        var result = underlying.Seek(position);
        OnSeek?.Invoke(current, position);
        return result;
    }

    public override void ResetSpeedAdjustments()
    {
        underlying.ResetSpeedAdjustments();
    }

    public override bool SeekForce(double position)
    {
        return underlying.Seek(position);
    }

    public override void ProcessFrame() { }

    public void ChangeSource(IClock source)
    {
        Track?.Expire();
        Track = new DrawableTrack(source as Track ?? tracks.GetVirtual(10000));
        Track?.AddAdjustment(AdjustableProperty.Frequency, RateBindable);
        underlying.ChangeSource(source);

        AddInternal(Track);
    }

    protected override void Update()
    {
        base.Update();

        updateStep();
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        Track?.Dispose();
    }

    #region Timing Stuff

    public double StepTime => stepTime / Rate;
    public double BeatTime => StepTime * 4;
    public Action<int> OnBeat { get; set; }

    private int lastStep;
    private int step;
    private float stepTime;

    private void updateStep()
    {
        lastStep = step;
        step = 0;
        stepTime = 1000;

        if (mapInfo == null) return;
        if (!mapInfo.TimingPoints.Any()) return;

        var point = mapInfo.GetTimingPoint(CurrentTime);

        stepTime = 60000f / point.BPM / point.Signature;

        var timeSinceTimingPoint = CurrentTime - point.Time;
        step = (int)(timeSinceTimingPoint / stepTime);

        if (lastStep != step && step % 4 == 0)
            OnBeat?.Invoke(step / 4);
    }

    #endregion
}
