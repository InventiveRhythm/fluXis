using System;
using System.Linq;
using System.Threading.Tasks;
using fluXis.Game.Configuration;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using fluXis.Game.Screens;
using fluXis.Game.Screens.Select;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Audio;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Transforms;
using osu.Framework.Timing;

namespace fluXis.Game.Audio;

public partial class GlobalClock : CompositeComponent, IAdjustableClock, IFrameBasedClock, ISourceChangeableClock, IBeatSyncProvider, IAmplitudeProvider
{
    [Resolved]
    private FluXisScreenStack screens { get; set; }

    [Resolved]
    private MapStore maps { get; set; }

    [Resolved]
    private ITrackStore tracks { get; set; }

    [CanBeNull]
    public DrawableTrack CurrentTrack => track.Value;

    public double ElapsedFrameTime => underlying.ElapsedFrameTime;
    public double FramesPerSecond => underlying.FramesPerSecond;
    public FrameTimeInfo TimeInfo => new() { Elapsed = ElapsedFrameTime, Current = CurrentTime };

    public double CurrentTime => underlying.CurrentTime - (offset?.Value ?? 0);
    public bool IsRunning => underlying.IsRunning;
    public bool Finished => CurrentTrack?.CurrentTime >= trackLength;
    public IClock Source => underlying.Source;

    public Bindable<double> RateBindable { get; } = new(1);

    public double Rate
    {
        get => RateBindable.Value;
        set => RateBindable.Value = value;
    }

    public bool Looping
    {
        get => CurrentTrack?.Looping ?? false;
        set
        {
            if (CurrentTrack == null)
                return;

            CurrentTrack.Looping = value;
        }
    }

    public double RestartPoint
    {
        get => CurrentTrack?.RestartPoint ?? 0;
        set
        {
            if (CurrentTrack == null)
                return;

            CurrentTrack.Looping = true;
            CurrentTrack.RestartPoint = value;
        }
    }

    public LowPassFilter LowPassFilter { get; private set; }

    private string trackPath;

    [CanBeNull]
    private MapInfo mapInfo { get; set; }

    private FramedMapClock underlying { get; }
    private Bindable<DrawableTrack> track { get; } = new();
    private Bindable<float> offset;

    private double trackLength => CurrentTrack?.Length ?? 10000;

    private const int limited_loop_time = 15000;
    private const int limited_loop_fade = 2000;
    private const int limited_loop_fade_in = 1000;
    private Bindable<LoopMode> loopMode;
    private bool loopEndReached;
    public bool AllowLimitedLoop { get; set; } = true;

    public GlobalClock()
    {
        underlying = new FramedMapClock();
        AddInternal(underlying);
    }

    [BackgroundDependencyLoader]
    private void load(AudioManager manager, FluXisConfig config)
    {
        AddInternal(LowPassFilter = new LowPassFilter(manager.TrackMixer));
        offset = config.GetBindable<float>(FluXisSetting.GlobalOffset);
        loopMode = config.GetBindable<LoopMode>(FluXisSetting.LoopMode);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        maps.MapBindable.BindValueChanged(onMapChange);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        CurrentTrack?.Dispose();
        maps.MapBindable.ValueChanged -= onMapChange;
    }

    private void onMapChange(ValueChangedEvent<RealmMap> e)
    {
        var newPath = e.NewValue?.MapSet.GetPathForFile(e.NewValue.Metadata.Audio);

        if (newPath == trackPath) return;

        LoadMap(e.NewValue);
        trackPath = newPath;

        if (screens.CurrentScreen is SelectScreen) Seek(e.NewValue?.Metadata.PreviewTime ?? 0);
    }

    public void LoadMap(RealmMap info)
    {
        // reset stuff
        AllowLimitedLoop = true;
        mapInfo = null;

        ChangeSource(info.GetTrack() ?? tracks.GetVirtual());
        Seek(0);
        Start();

        Task.Run(() => mapInfo = info.GetMapInfo());
    }

    public void Reset()
    {
        ClearTransforms();
        underlying.Reset();
        CurrentTrack?.Reset();
    }

    public void Start()
    {
        underlying.Start();
        CurrentTrack?.Start();
    }

    public void Stop()
    {
        underlying.Stop();
        CurrentTrack?.Stop();
    }

    public bool Seek(double position)
    {
        position = Math.Min(position, trackLength);
        return underlying.Seek(position);
    }

    public void ResetSpeedAdjustments()
    {
        Schedule(() => ClearTransforms());
        underlying.ResetSpeedAdjustments();
        CurrentTrack?.ResetSpeedAdjustments();
    }

    public void ProcessFrame() { }

    public void ChangeSource(IClock source)
    {
        const float crossfade_duration = 400;

        var current = CurrentTrack;
        current?.VolumeTo(0, crossfade_duration, Easing.Out).Expire();

        if (source is Track t)
        {
            track.Value = new DrawableTrack(t);
            AddInternal(CurrentTrack);
        }

        underlying.ChangeSource(source);

        if (CurrentTrack == null)
            return;

        CurrentTrack.AddAdjustment(AdjustableProperty.Frequency, RateBindable);
        CurrentTrack.VolumeTo(0).VolumeTo(1, crossfade_duration, Easing.Out);
    }

    protected override void Update()
    {
        base.Update();

        if (Looping && AllowLimitedLoop && CurrentTrack?.CurrentTime - RestartPoint >= limited_loop_time - limited_loop_fade)
        {
            if (!loopEndReached && loopMode.Value == LoopMode.Limited)
            {
                CurrentTrack.VolumeTo(0, limited_loop_fade).OnComplete(track =>
                {
                    // might have changed who knows
                    if (CurrentTrack != track)
                        return;

                    Seek(RestartPoint);
                    track.VolumeTo(1, limited_loop_fade_in);
                });
                loopEndReached = true;
            }
        }
        else loopEndReached = false;

        updateStep();
        updateAmplitudes();
    }

    #region Step Stuff

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

    #region Amplitude Stuff

    public float[] Amplitudes { get; } = new float[256];
    private double lastAmplitudeUpdate;
    private const int amplitude_update_fps = 120;

    private void updateAmplitudes()
    {
        if (CurrentTrack == null) return;

        if (Time.Current - lastAmplitudeUpdate < 1000f / amplitude_update_fps)
            return;

        var elapsed = Time.Current - lastAmplitudeUpdate;
        lastAmplitudeUpdate = Time.Current;

        ReadOnlySpan<float> span = null;

        if (IsRunning)
            span = CurrentTrack.CurrentAmplitudes.FrequencyAmplitudes.Span;

        if (span == null) span = new float[256];

        for (var i = 0; i < span.Length; i++)
        {
            float newAmplitude = span[i];
            float delta = newAmplitude - Amplitudes[i];
            float interpolation = delta < 0
                ? (float)elapsed / 30f
                : (float)Math.Pow(.1f, elapsed / 1000f);

            Amplitudes[i] += delta * interpolation;
        }
    }

    #endregion
}

public static class GlobalClockExtensions
{
    public static TransformSequence<DrawableTrack> VolumeTo(this GlobalClock clock, double newVolume, double duration = 0, Easing easing = Easing.None)
        => clock.CurrentTrack?.VolumeTo(newVolume, duration, easing);

    public static TransformSequence<DrawableTrack> VolumeIn(this GlobalClock clock, double duration = 0, Easing easing = Easing.None)
        => clock.VolumeTo(1f, duration, easing);

    public static TransformSequence<DrawableTrack> VolumeOut(this GlobalClock clock, double duration = 0, Easing easing = Easing.None)
        => clock.VolumeTo(0f, duration, easing);

    public static TransformSequence<GlobalClock> RateTo(this GlobalClock clock, double newRate, double duration = 0, Easing easing = Easing.None)
        => clock.TransformBindableTo(clock.RateBindable, newRate, duration, easing);
}
