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

    public void Add(TimedObject obj)
    {
        switch (obj)
        {
            case HitObject hit:
                HitObjects.Add(hit);
                HitObjectAdded?.Invoke(hit);
                break;

            case TimingPoint timing:
                TimingPoints.Add(timing);
                TimingPointAdded?.Invoke(timing);
                break;

            case ScrollVelocity sv:
                ScrollVelocities.Add(sv);
                ScrollVelocityAdded?.Invoke(sv);
                break;

            default:
                throw new NotImplementedException();
        }
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

    public void Remove(TimedObject obj)
    {
        switch (obj)
        {
            case HitObject hit:
                HitObjects.Remove(hit);
                HitObjectRemoved?.Invoke(hit);
                break;

            case TimingPoint timing:
                TimingPoints.Remove(timing);
                TimingPointRemoved?.Invoke(timing);
                break;

            case ScrollVelocity sv:
                ScrollVelocities.Remove(sv);
                ScrollVelocityRemoved?.Invoke(sv);
                break;

            default:
                throw new NotImplementedException();
        }
    }

    public void ChangeHitSounds() => HitSoundsChanged?.Invoke();

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
