using System;
using fluXis.Game.Map;
using fluXis.Game.Map.Structures;
using Newtonsoft.Json;

namespace fluXis.Game.Screens.Edit;

public class EditorMapInfo : MapInfo
{
    [JsonIgnore]
    public EditorMapEvents MapEvents { get; set; } = new();

    public event Action<HitObject> HitObjectAdded;
    public event Action<HitObject> HitObjectRemoved;
    public event Action<HitObject> HitObjectChanged;
    public event Action HitSoundsChanged;

    public event Action<TimingPoint> TimingPointAdded;
    public event Action<TimingPoint> TimingPointRemoved;
    public event Action<TimingPoint> TimingPointChanged;

    public event Action<ScrollVelocity> ScrollVelocityAdded;
    public event Action<ScrollVelocity> ScrollVelocityRemoved;
    public event Action<ScrollVelocity> ScrollVelocityChanged;

    public EditorMapInfo(MapMetadata metadata)
        : base(metadata)
    {
    }

    public static EditorMapInfo FromMapInfo(MapInfo info)
    {
        return new EditorMapInfo(info.Metadata)
        {
            AudioFile = info.AudioFile,
            BackgroundFile = info.BackgroundFile,
            CoverFile = info.CoverFile,
            VideoFile = info.VideoFile,
            EffectFile = info.EffectFile,
            HitObjects = info.HitObjects,
            TimingPoints = info.TimingPoints,
            ScrollVelocities = info.ScrollVelocities,
            InitialKeyCount = info.InitialKeyCount,
            AccuracyDifficulty = info.AccuracyDifficulty,
            MapEvents = info.GetMapEvents<EditorMapEvents>()
        };
    }

    public void Update(TimedObject obj)
    {
        switch (obj)
        {
            case HitObject hit:
                HitObjectChanged?.Invoke(hit);
                break;

            case TimingPoint timing:
                TimingPointChanged?.Invoke(timing);
                break;

            case ScrollVelocity sv:
                ScrollVelocityChanged?.Invoke(sv);
                break;

            default:
                MapEvents.Update(obj);
                break;
        }
    }

    public void Add(HitObject hitObject)
    {
        HitObjects.Add(hitObject);
        HitObjectAdded?.Invoke(hitObject);
    }

    public void Remove(HitObject hitObject)
    {
        HitObjects.Remove(hitObject);
        HitObjectRemoved?.Invoke(hitObject);
    }

    public void ChangeHitSounds() =>
        HitSoundsChanged?.Invoke();

    public void Add(TimingPoint timingPoint)
    {
        TimingPoints.Add(timingPoint);
        TimingPointAdded?.Invoke(timingPoint);
    }

    public void Remove(TimingPoint timingPoint)
    {
        TimingPoints.Remove(timingPoint);
        TimingPointRemoved?.Invoke(timingPoint);
    }

    public void Change(TimingPoint timingPoint)
    {
        TimingPointChanged?.Invoke(timingPoint);
    }

    public void Add(ScrollVelocity scrollVelocity)
    {
        ScrollVelocities.Add(scrollVelocity);
        ScrollVelocityAdded?.Invoke(scrollVelocity);
    }

    public void Remove(ScrollVelocity scrollVelocity)
    {
        ScrollVelocities.Remove(scrollVelocity);
        ScrollVelocityRemoved?.Invoke(scrollVelocity);
    }

    public void Change(ScrollVelocity scrollVelocity)
    {
        ScrollVelocityChanged?.Invoke(scrollVelocity);
    }

    public override T GetMapEvents<T>() => MapEvents as T;

    public void ApplyOffsetToAll(float offset)
    {
        foreach (var hitObject in HitObjects)
        {
            hitObject.Time += offset;
            Update(hitObject);
        }

        foreach (var timingPoint in TimingPoints)
        {
            timingPoint.Time += offset;
            Update(timingPoint);
        }

        foreach (var scrollVelocity in ScrollVelocities)
        {
            scrollVelocity.Time += offset;
            Update(scrollVelocity);
        }

        MapEvents.ApplyOffsetToAll(offset);
    }

    public new EditorMapInfo Clone()
    {
        return new EditorMapInfo(Metadata)
        {
            AudioFile = AudioFile,
            BackgroundFile = BackgroundFile,
            CoverFile = CoverFile,
            VideoFile = VideoFile,
            EffectFile = EffectFile,
            HitObjects = HitObjects,
            TimingPoints = TimingPoints,
            ScrollVelocities = ScrollVelocities,
            InitialKeyCount = InitialKeyCount,
            Map = Map,
            MapEvents = MapEvents
        };
    }
}
