using System;
using System.Collections.Generic;
using fluXis.Game.Overlay.Notification;
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
    public List<HitObjectInfo> HitObjects;
    public List<TimingPointInfo> TimingPoints;
    public List<ScrollVelocityInfo> ScrollVelocities;

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
    public int KeyCount;

    [JsonIgnore]
    public int InitialKeyCount;

    public MapInfo(MapMetadata metadata)
    {
        Metadata = metadata;
        HitObjects = new List<HitObjectInfo>();
        TimingPoints = new List<TimingPointInfo>();
        ScrollVelocities = new List<ScrollVelocityInfo>();

        // Add default timing point so it doesn't crash
        TimingPoints.Add(new TimingPointInfo { BPM = 120, Time = 0, Signature = 4 });
    }

    public bool Validate(NotificationOverlay notifications = null)
    {
        if (HitObjects.Count == 0)
        {
            notifications?.PostError("Map has no hit objects");
            return false;
        }

        if (TimingPoints.Count == 0)
        {
            notifications?.PostError("Map has no timing points");
            return false;
        }

        foreach (var hitObject in HitObjects)
        {
            KeyCount = Math.Max(KeyCount, hitObject.Lane);
        }

        foreach (var timingPoint in TimingPoints)
        {
            if (timingPoint.BPM <= 0)
            {
                notifications?.PostError("A timing point has an invalid BPM");
                return false;
            }

            if (timingPoint.Signature <= 0)
            {
                notifications?.PostError("A timing point has an invalid signature");
                return false;
            }
        }

        if (KeyCount is <= 0 or >= 11)
        {
            notifications?.PostError("Map has an invalid key count");
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
            VideoFile = VideoFile,
            EffectFile = EffectFile,
            HitObjects = HitObjects,
            TimingPoints = TimingPoints,
            ScrollVelocities = ScrollVelocities,
            KeyCount = KeyCount,
            InitialKeyCount = InitialKeyCount
        };
    }
}
