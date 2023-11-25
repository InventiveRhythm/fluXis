using System;
using System.Collections.Generic;
using fluXis.Game.Map;
using fluXis.Game.Map.Events;
using Newtonsoft.Json;

namespace fluXis.Game.Screens.Edit;

public class EditorMapInfo : MapInfo
{
    [JsonIgnore]
    public EditorMapEvents MapEvents { get; set; } = new();

    public event Action<HitObjectInfo> HitObjectAdded;
    public event Action<HitObjectInfo> HitObjectRemoved;

    public event Action<TimingPointInfo> TimingPointAdded;
    public event Action<TimingPointInfo> TimingPointRemoved;
    public event Action<TimingPointInfo> TimingPointChanged;

    public event Action<ScrollVelocityInfo> ScrollVelocityAdded;
    public event Action<ScrollVelocityInfo> ScrollVelocityRemoved;
    public event Action<ScrollVelocityInfo> ScrollVelocityChanged;

    public EditorMapInfo(MapMetadata metadata)
        : base(metadata)
    {
    }

    public static EditorMapInfo FromMapInfo(MapInfo info)
    {
        var events = info.GetMapEvents();

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
            MapEvents = new EditorMapEvents
            {
                LaneSwitchEvents = events.LaneSwitchEvents ?? new List<LaneSwitchEvent>(),
                FlashEvents = events.FlashEvents ?? new List<FlashEvent>(),
                PulseEvents = events.PulseEvents ?? new List<PulseEvent>(),
                PlayfieldMoveEvents = events.PlayfieldMoveEvents ?? new List<PlayfieldMoveEvent>(),
                PlayfieldScaleEvents = events.PlayfieldScaleEvents ?? new List<PlayfieldScaleEvent>(),
                ShakeEvents = events.ShakeEvents ?? new List<ShakeEvent>(),
                PlayfieldFadeEvents = events.PlayfieldFadeEvents ?? new List<PlayfieldFadeEvent>()
            }
        };
    }

    public void Add(HitObjectInfo hitObject)
    {
        HitObjects.Add(hitObject);
        HitObjectAdded?.Invoke(hitObject);
    }

    public void Remove(HitObjectInfo hitObject)
    {
        HitObjects.Remove(hitObject);
        HitObjectRemoved?.Invoke(hitObject);
    }

    public void Add(TimingPointInfo timingPoint)
    {
        TimingPoints.Add(timingPoint);
        TimingPointAdded?.Invoke(timingPoint);
    }

    public void Remove(TimingPointInfo timingPoint)
    {
        TimingPoints.Remove(timingPoint);
        TimingPointRemoved?.Invoke(timingPoint);
    }

    public void Change(TimingPointInfo timingPoint)
    {
        TimingPointChanged?.Invoke(timingPoint);
    }

    public void Add(ScrollVelocityInfo scrollVelocity)
    {
        ScrollVelocities.Add(scrollVelocity);
        ScrollVelocityAdded?.Invoke(scrollVelocity);
    }

    public void Remove(ScrollVelocityInfo scrollVelocity)
    {
        ScrollVelocities.Remove(scrollVelocity);
        ScrollVelocityRemoved?.Invoke(scrollVelocity);
    }

    public void Change(ScrollVelocityInfo scrollVelocity)
    {
        ScrollVelocityChanged?.Invoke(scrollVelocity);
    }

    public override MapEvents GetMapEvents() => MapEvents;

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
