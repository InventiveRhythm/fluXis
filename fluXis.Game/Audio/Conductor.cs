using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Transforms;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;

namespace fluXis.Game.Audio;

/// <inheritdoc cref="osu.Framework.Graphics.Containers.Container" />
/// <summary>
/// A class that handles all music and timing related things.
/// </summary>
public partial class Conductor : Container
{
    private static readonly BindableNumber<double> bind_speed = new BindableDouble(1);
    private static Conductor instance;
    private static ITrackStore trackStore;
    private static Storage storage;

    /// <summary>
    /// Current time of the track in milliseconds.
    /// </summary>
    public static float CurrentTime;

    /// <summary>
    /// Offset of the track in milliseconds.
    /// </summary>
    public static int Offset = 0;

    /// <summary>
    /// The low pass filter for all tracks.
    /// </summary>
    public static LowPassFilter LowPassFilter;

    /// <summary>
    /// The speed of the track. 1 is normal speed.
    /// </summary>
    public static float Speed
    {
        get => instance.untweenedSpeed;
        set => SetSpeed(value);
    }

    /// <summary>
    /// Returns whether the track is playing or not.
    /// </summary>
    public static bool IsPlaying => Track?.IsRunning ?? false;

    /// <summary>
    /// Returns whether the track has finished playing or not.
    /// </summary>
    public static bool HasFinished => Track?.HasCompleted ?? false;

    /// <summary>
    /// Returns the length of the track in milliseconds.
    /// </summary>
    public static float Length
    {
        get
        {
            if (Track == null)
                return 0;

            return (float)Track.Length;
        }
    }

    /// <summary>
    /// The current Track itself.
    /// </summary>
    public static Track Track { get; private set; }

    private float speed = 1;
    private float untweenedSpeed = 1;
    private float trackVolume = 1f;

    /// <summary>
    /// A callback that is invoked every beat.
    /// </summary>
    public static Action<int> OnBeat;

    /// <summary>
    /// The time between each step in milliseconds.
    /// </summary>
    public static float StepTime => instance.stepTime;

    /// <summary>
    /// The time between each beat in milliseconds.
    /// </summary>
    public static float BeatTime => instance.stepTime * 4;

    public static List<TimingPointInfo> TimingPoints = new();

    [BackgroundDependencyLoader]
    private void load(AudioManager audioManager, Storage storage)
    {
        instance = this;
        Conductor.storage = storage;
        trackStore = audioManager.GetTrackStore(new StorageBackedResourceStore(storage.GetStorageForDirectory("files")));
        Add(LowPassFilter = new LowPassFilter(audioManager.TrackMixer));
    }

    protected override void Update()
    {
        bind_speed.Value = speed;

        if (Track != null) Track.Volume.Value = trackVolume;

        if (CurrentTime < 0)
        {
            CurrentTime += (float)Time.Elapsed;
            return;
        }

        if (Track != null)
            CurrentTime = (float)Track.CurrentTime + Offset;
        else
            CurrentTime += (float)Time.Elapsed + Offset;

        updateStep();
    }

    /// <summary>
    /// Start playing a track from a map.
    /// </summary>
    /// <param name="info">The map to play the track of</param>
    /// <param name="start">Whether to start the track immediately</param>
    /// <param name="time">The time to start the track at (used for preview in song select)</param>
    public static void PlayTrack(RealmMap info, bool start = false, float time = 0)
    {
        StopTrack();

        Track = trackStore.Get(info.MapSet.GetFile(info.Metadata.Audio)?.GetPath()) ?? trackStore.GetVirtual();
        Track.AddAdjustment(AdjustableProperty.Frequency, bind_speed);

        Track.Seek(time);

        if (start)
            Track.Start();

        TimingPoints = MapUtils.LoadFromPath(storage.GetFullPath("files/" + PathUtils.HashToPath(info.Hash)))?.TimingPoints ?? new List<TimingPointInfo>();
    }

    /// <summary>
    /// Seeks the track to the specified time.
    /// </summary>
    /// <param name="time">The time to seek to.</param>
    public static void Seek(float time)
    {
        Track?.Seek(time);
    }

    /// <summary>
    /// Pauses the track if it is playing.
    /// </summary>
    public static void PauseTrack()
    {
        Track?.Stop();
    }

    /// <summary>
    /// Resumes the track if it was paused.
    /// </summary>
    public static void ResumeTrack()
    {
        Track?.Start();
    }

    /// <summary>
    /// Completely stops the track and disposes it.
    /// </summary>
    public static void StopTrack()
    {
        Track?.Stop();
        Track?.Dispose();
        Track = null;
        TimingPoints.Clear();
    }

    /// <summary>
    /// Sets the track to loop from the specified time.
    /// </summary>
    /// <param name="start">The time to start the loop at.</param>
    public static void SetLoop(float start)
    {
        if (Track == null)
            return;

        Track.Looping = true;
        Track.RestartPoint = start;
    }

    /// <summary>
    /// Resets the track's loop.
    /// </summary>
    public static void ResetLoop()
    {
        if (Track == null)
            return;

        Track.Looping = false;
    }

    /// <summary>
    /// Transforms the speed of the track.
    /// </summary>
    /// <param name="newSpeed">The speed to transform to.</param>
    /// <param name="duration">The transform duration.</param>
    /// <param name="ease">The transform easing used for tweening.</param>
    /// <returns></returns>
    public static TransformSequence<Conductor> SetSpeed(float newSpeed, int duration = 400, Easing ease = Easing.OutQuint)
    {
        instance.untweenedSpeed = newSpeed;
        return instance.TransformTo(nameof(speed), newSpeed, duration, ease);
    }

    public static void FadeOut(int duration = 400)
    {
        instance.TransformTo(nameof(trackVolume), 0f, duration);
    }

    private int lastStep;
    private int step;
    private float stepTime;

    // gonna be used somewhere later
    private void updateStep()
    {
        lastStep = step;
        step = 0;
        stepTime = 0;

        if (!TimingPoints.Any())
            return;

        var point = TimingPoints.LastOrDefault(p => p.Time <= CurrentTime) ?? TimingPoints.First();

        stepTime = 60000f / point.BPM / point.Signature;
        lastStep = step;
        step = (int)(CurrentTime / stepTime);

        if (lastStep != step && step % 4 == 0)
            OnBeat?.Invoke(step / 4);
    }
}
