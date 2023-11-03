using System.Collections.Generic;
using System.IO;
using System.Linq;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Overlay.Notifications;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace fluXis.Game.Map;

public class MapInfo
{
    public string AudioFile { get; set; } = string.Empty;
    public string BackgroundFile { get; set; } = string.Empty;
    public string CoverFile { get; set; } = string.Empty;
    public string VideoFile { get; set; } = string.Empty;
    public string EffectFile { get; set; } = string.Empty;
    public MapMetadata Metadata { get; set; }
    public List<HitObjectInfo> HitObjects { get; set; }
    public List<TimingPointInfo> TimingPoints { get; set; }
    public List<ScrollVelocityInfo> ScrollVelocities { get; set; }

    [JsonIgnore]
    public float StartTime => HitObjects[0].Time;

    [JsonIgnore]
    public float EndTime => HitObjects[^1].HoldEndTime;

    [JsonIgnore]
    public int MaxCombo
    {
        get
        {
            int maxCombo = 0;

            foreach (var hitObject in HitObjects)
            {
                maxCombo++;
                if (hitObject.IsLongNote())
                    maxCombo++;
            }

            return maxCombo;
        }
    }

    [JsonIgnore]
    public int InitialKeyCount { get; set; }

    [CanBeNull]
    [JsonIgnore]
    public RealmMap Map { get; set; }

    [JsonIgnore]
    public string Hash { get; set; }

    public MapInfo(MapMetadata metadata)
    {
        Metadata = metadata;
        HitObjects = new List<HitObjectInfo>();
        TimingPoints = new List<TimingPointInfo> { new() { BPM = 120, Time = 0, Signature = 4 } }; // Add default timing point to avoid issues
        ScrollVelocities = new List<ScrollVelocityInfo>();
    }

    public bool Validate(NotificationManager notifications = null)
    {
        if (HitObjects.Count == 0)
        {
            notifications?.SendError("Map has no hit objects");
            return false;
        }

        if (TimingPoints.Count == 0)
        {
            notifications?.SendError("Map has no timing points");
            return false;
        }

        foreach (var timingPoint in TimingPoints)
        {
            if (timingPoint.BPM <= 0)
            {
                notifications?.SendError("A timing point has an invalid BPM");
                return false;
            }

            if (timingPoint.Signature <= 0)
            {
                notifications?.SendError("A timing point has an invalid signature");
                return false;
            }
        }

        if (HitObjects.Any(hitObject => hitObject.Lane < 1))
        {
            notifications?.SendError("Map has an invalid key count");
            return false;
        }

        return true;
    }

    public void Sort()
    {
        HitObjects.Sort((a, b) => a.Time == b.Time ? a.Lane.CompareTo(b.Lane) : a.Time.CompareTo(b.Time));
        TimingPoints.Sort((a, b) => a.Time.CompareTo(b.Time));
        ScrollVelocities?.Sort((a, b) => a.Time.CompareTo(b.Time));
    }

    public virtual MapEvents GetMapEvents()
    {
        var events = new MapEvents();
        if (Map == null) return events;

        var effectFile = Map.MapSet.GetPathForFile(EffectFile);
        if (string.IsNullOrEmpty(effectFile)) return events;

        if (!File.Exists(MapFiles.GetFullPath(effectFile))) return events;

        var content = File.ReadAllText(MapFiles.GetFullPath(effectFile));
        events.Load(content);
        return events;
    }

    public TimingPointInfo GetTimingPoint(double time)
    {
        if (TimingPoints.Count == 0)
            return new TimingPointInfo { BPM = 60, Time = 0 };

        TimingPointInfo timingPoint = null;

        foreach (var tp in TimingPoints)
        {
            if (tp.Time > time)
                break;

            timingPoint = tp;
        }

        return timingPoint ?? TimingPoints[0];
    }

    public MapInfo Clone()
    {
        return new MapInfo(Metadata)
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
            Map = Map
        };
    }
}
