using fluXis.Game.Audio.Transforms;
using fluXis.Game.Configuration;
using fluXis.Game.Map;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Timing;

namespace fluXis.Game.Screens.Gameplay.Audio;

public partial class GameplayClock : TransformableClock, IFrameBasedClock, ISourceChangeableClock
{
    public float Offset => useOffset ? offset.Value : 0;
    private bool useOffset { get; }

    public override double CurrentTime => underlying.CurrentTime - Offset;
    public override bool IsRunning => underlying.IsRunning;
    public double ElapsedFrameTime => underlying.ElapsedFrameTime;
    public double FramesPerSecond => underlying.FramesPerSecond;
    public IClock Source => underlying.Source;

    private MapInfo mapInfo;
    private Track track;

    private FramedMapClock underlying { get; }
    private Bindable<float> offset;

    public GameplayClock(MapInfo info, Track track, bool useOffset)
    {
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
        return underlying.Seek(position);
    }

    public override void ResetSpeedAdjustments()
    {
        underlying.ResetSpeedAdjustments();
    }

    public override bool SeekForce(double position)
    {
        return underlying.Seek(position);
    }

    public void ProcessFrame() { }

    public void ChangeSource(IClock source)
    {
        track?.Dispose();
        track = source as Track;
        track?.AddAdjustment(AdjustableProperty.Frequency, RateBindable);
        underlying.ChangeSource(source);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        track?.Dispose();
    }

    #region Timing Stuff

    private double stepTime
    {
        get
        {
            var point = mapInfo.GetTimingPoint(CurrentTime);
            return 60000f / point.BPM / point.Signature;
        }
    }

    public double StepTime => stepTime / Rate;
    public double BeatTime => StepTime * 4;

    #endregion
}
