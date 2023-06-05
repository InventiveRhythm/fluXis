using System;
using System.Linq;
using System.Threading.Tasks;
using fluXis.Game.Audio.Transforms;
using fluXis.Game.Configuration;
using fluXis.Game.Database.Maps;
using fluXis.Game.Import;
using fluXis.Game.Map;
using fluXis.Game.Utils;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osu.Framework.Timing;

namespace fluXis.Game.Audio;

public partial class AudioClock : TransformableClock, IFrameBasedClock, ISourceChangeableClock
{
    [Resolved]
    private AudioManager audioManager { get; set; }

    [Resolved]
    private ITrackStore trackStore { get; set; }

    [Resolved]
    private ImportManager importManager { get; set; }

    private ITrackStore realmTrackStore { get; set; }
    private Storage realmStorage { get; set; }

    public IBindable<Track> Track => track;
    public float TrackLength => (float)(track.Value?.Length ?? 10000);

    [CanBeNull]
    public MapInfo MapInfo { get; set; }

    public double ElapsedFrameTime => underlying.ElapsedFrameTime;
    public double FramesPerSecond => underlying.FramesPerSecond;
    public FrameTimeInfo TimeInfo => underlying.TimeInfo;

    public override double CurrentTime => underlying.CurrentTime + offset.Value;
    public IClock Source => underlying.Source;
    public override bool IsRunning => underlying.IsRunning;
    double IClock.Rate => underlying.Rate;

    public bool Finished => CurrentTime >= TrackLength;

    public bool Looping
    {
        get => track.Value.Looping;
        set => track.Value.Looping = value;
    }

    public double RestartPoint
    {
        get => track.Value.RestartPoint;
        set
        {
            track.Value.Looping = true;
            track.Value.RestartPoint = value;
        }
    }

    public LowPassFilter LowPassFilter { get; private set; }

    private readonly FramedMapClock underlying;
    private readonly Bindable<Track> track = new();
    private Bindable<float> offset;

    public AudioClock()
    {
        underlying = new FramedMapClock { IsCoupled = false };
        AddInternal(underlying);
    }

    [BackgroundDependencyLoader]
    private void load(Storage storage, FluXisConfig config)
    {
        realmStorage = storage.GetStorageForDirectory("files");
        realmTrackStore = audioManager.GetTrackStore(new StorageBackedResourceStore(realmStorage));
        AddInternal(LowPassFilter = new LowPassFilter(audioManager.TrackMixer));
        offset = config.GetBindable<float>(FluXisSetting.GlobalOffset);
    }

    public void LoadMap(RealmMap info, bool start = false, bool usePreview = false)
    {
        Stop();

        Track newTrack;

        if (info.MapSet.Managed)
        {
            string path = importManager.GetAsset(info, ImportedAssetType.Audio);
            newTrack = importManager.GetTrackStore(info.Status)?.Get(path);
        }
        else
        {
            string path = info.MapSet.GetFile(info.Metadata.Audio)?.GetPath();
            newTrack = realmTrackStore.Get(path);
        }

        ChangeSource(newTrack ?? realmTrackStore.GetVirtual());

        Seek(usePreview ? info.Metadata.PreviewTime : 0);

        if (start)
            Start();

        Task.Run(() =>
        {
            MapInfo = null;
            MapInfo = MapUtils.LoadFromPath(realmStorage.GetFullPath(PathUtils.HashToPath(info.Hash)));
        });
    }

    public void PlayTrack(string path, bool start = false)
    {
        Stop();
        ChangeSource(trackStore.Get(path));
        if (start) Start();
    }

    public override void Reset()
    {
        ClearTransforms();
        underlying.Reset();
    }

    public override void Start()
    {
        Schedule(() => ClearTransforms());
        underlying.Start();
    }

    public override void Stop() => underlying.Stop();

    public override bool Seek(double position)
    {
        Schedule(() => ClearTransforms());
        position = Math.Min(position, TrackLength);
        return underlying.Seek(position);
    }

    public override bool SeekForce(double position) => underlying.Seek(position);

    public void SeekSmoothly(double time)
    {
        if (IsRunning)
            Seek(time);
        else
            TimeTo(time, 300, Easing.OutQuint);
    }

    public override void ResetSpeedAdjustments()
    {
        Schedule(() => ClearTransforms());
        underlying.ResetSpeedAdjustments();
    }

    public void ProcessFrame() { }

    public void ChangeSource(IClock source)
    {
        track.Value = source as Track;
        Track.Value.AddAdjustment(AdjustableProperty.Frequency, RateBindable);
        underlying.ChangeSource(source);
    }

    protected override void Update()
    {
        base.Update();

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

        if (MapInfo == null) return;
        if (!MapInfo.TimingPoints.Any()) return;

        var point = MapInfo.GetTimingPoint(CurrentTime);

        stepTime = 60000f / point.BPM / point.Signature;
        step = (int)(CurrentTime / stepTime);

        if (lastStep != step && step % 4 == 0)
            OnBeat?.Invoke(step / 4);
    }

    #endregion

    #region Amplitude Stuff

    public float[] Amplitudes { get; private set; } = new float[256];
    private double lastAmplitudeUpdate;
    private const int amplitude_update_fps = 120;

    private void updateAmplitudes()
    {
        if (Time.Current - lastAmplitudeUpdate < 1000f / amplitude_update_fps)
            return;

        lastAmplitudeUpdate = Time.Current;

        ReadOnlySpan<float> span = new float[256];

        if (Track != null && IsRunning)
            span = Track.Value.CurrentAmplitudes.FrequencyAmplitudes.Span;

        for (var i = 0; i < span.Length; i++)
        {
            float newAmplitude = span[i];
            float delta = newAmplitude - Amplitudes[i];
            float interpolation = delta < 0
                ? (float)Time.Elapsed / 30f
                : (float)Math.Pow(.1f, Time.Elapsed / 1000f);

            Amplitudes[i] += delta * interpolation;
        }
    }

    #endregion

    #region Volume

    public Bindable<double> VolumeBindable => Track.Value.Volume;

    public void FadeTo(double volume, double duration = 0, Easing easing = Easing.None) => this.TransformBindableTo(VolumeBindable, volume, duration, easing);
    public void FadeIn(double duration = 0, Easing easing = Easing.None) => FadeTo(1, duration, easing);
    public void FadeOut(double duration = 0, Easing easing = Easing.None) => FadeTo(0, duration, easing);

    #endregion
}
