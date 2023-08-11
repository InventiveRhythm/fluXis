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
            KeyCount = info.KeyCount,
            InitialKeyCount = info.InitialKeyCount,
            MapEvents = new EditorMapEvents
            {
                LaneSwitchEvents = events.LaneSwitchEvents ?? new List<LaneSwitchEvent>(),
                FlashEvents = events.FlashEvents ?? new List<FlashEvent>(),
                PulseEvents = events.PulseEvents ?? new List<PulseEvent>(),
                PlayfieldMoveEvents = events.PlayfieldMoveEvents ?? new List<PlayfieldMoveEvent>()
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
            KeyCount = KeyCount,
            InitialKeyCount = InitialKeyCount,
            Map = Map,
            MapEvents = MapEvents
        };
    }
}
