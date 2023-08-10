using System;
using fluXis.Game.Map;
using Newtonsoft.Json;

namespace fluXis.Game.Screens.Edit;

public class EditorMapInfo : MapInfo
{
    [JsonIgnore]
    public MapEvents MapEvents { get; set; }

    public event Action<HitObjectInfo> HitObjectAdded;
    public event Action<HitObjectInfo> HitObjectRemoved;

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
            KeyCount = info.KeyCount,
            InitialKeyCount = info.InitialKeyCount,
            MapEvents = info.GetMapEvents()
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
