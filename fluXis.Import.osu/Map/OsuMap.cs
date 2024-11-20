using System.Collections.Generic;
using System.Linq;
using fluXis.Import.osu.Map.Components;
using fluXis.Game.Map;
using fluXis.Game.Map.Structures;
using fluXis.Import.osu.AutoImport;
using JetBrains.Annotations;

namespace fluXis.Import.osu.Map;

public class OsuMap
{
    // [General]
    public string AudioFilename { get; set; }
    public int PreviewTime { get; set; }
    public int Mode { get; set; }

    // [Editor]
    // uuhhhh

    // [Metadata]
    public string Title { get; set; }
    public string Artist { get; set; }
    public string Creator { get; set; }
    public string Version { get; set; }
    public string Source { get; set; }
    public string Tags { get; set; }

    // [Difficulty]
    public float CircleSize { get; set; }
    public float OverallDifficulty { get; set; }

    // [Events]
    public List<OsuEvent> Events { get; init; } = new();

    // [TimingPoints]
    public List<OsuTimingPoint> TimingPoints { get; init; } = new();

    // [Colours]
    // dont need this

    // [HitObjects]
    public List<OsuHitObject> HitObjects { get; init; } = new();

    public string GetBackground()
    {
        foreach (var osuEvent in Events)
        {
            string param = osuEvent.Parameter.Trim().Replace("\"", "");

            switch (osuEvent.EventType)
            {
                case "0":
                    return param;
            }
        }

        return "";
    }

    [CanBeNull]
    public OsuMapInfo ToMapInfo()
    {
        if (Mode != 3)
            return null;

        var mapInfo = new OsuMapInfo
        {
            Metadata = new MapMetadata
            {
                Title = Title?.Trim() ?? "",
                Artist = Artist?.Trim() ?? "",
                Mapper = Creator?.Trim() ?? "",
                Difficulty = Version?.Trim() ?? "",
                AudioSource = Source?.Trim() ?? "",
                Tags = Tags?.Trim() ?? "",
                PreviewTime = PreviewTime
            },
            AudioFile = AudioFilename?.Trim(),
            BackgroundFile = "",
            HitObjects = new List<HitObject>(),
            TimingPoints = new List<TimingPoint>(),
            ScrollVelocities = new List<ScrollVelocity>(),
            InitialKeyCount = (int)CircleSize,
            AccuracyDifficulty = OverallDifficulty
        };

        foreach (var timingPoint in TimingPoints)
        {
            if (timingPoint.IsScrollVelocity)
                mapInfo.ScrollVelocities.Add(timingPoint.ToScrollVelocityInfo());
            else
                mapInfo.TimingPoints.Add(timingPoint.ToTimingPointInfo());
        }

        mapInfo.HitObjects.AddRange(HitObjects.Select(h => h.ToHitObjectInfo(this)));

        foreach (var osuEvent in Events)
        {
            string param = osuEvent.Parameter.Trim().Replace("\"", "");

            switch (osuEvent.EventType)
            {
                case "0":
                    mapInfo.BackgroundFile = param;
                    break;

                case "Video":
                    mapInfo.VideoFile = param;
                    break;
            }
        }

        return mapInfo;
    }
}
