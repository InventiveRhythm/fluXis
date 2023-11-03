using System.Collections.Generic;
using fluXis.Import.osu.Map.Components;
using fluXis.Game.Map;

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

    public MapInfo ToMapInfo()
    {
        if (Mode != 3)
            throw new System.Exception("Only osu!mania maps are supported.");

        MapInfo mapInfo = new(new MapMetadata
        {
            Title = Title?.Trim() ?? "",
            Artist = Artist?.Trim() ?? "",
            Mapper = Creator?.Trim() ?? "",
            Difficulty = Version?.Trim() ?? "",
            Source = Source?.Trim() ?? "",
            Tags = Tags?.Trim() ?? "",
            PreviewTime = PreviewTime
        })
        {
            AudioFile = AudioFilename.Trim(),
            BackgroundFile = "",
            HitObjects = new List<HitObjectInfo>(),
            TimingPoints = new List<TimingPointInfo>(),
            ScrollVelocities = new List<ScrollVelocityInfo>(),
            InitialKeyCount = (int)CircleSize
        };

        List<int> keyCounts = new();

        foreach (var hitObject in HitObjects)
        {
            if (!keyCounts.Contains(hitObject.X))
                keyCounts.Add(hitObject.X);
        }

        foreach (var hitObject in HitObjects)
            mapInfo.HitObjects.Add(hitObject.ToHitObjectInfo(keyCounts.Count));

        foreach (var timingPoint in TimingPoints)
        {
            if (timingPoint.IsScrollVelocity)
                mapInfo.ScrollVelocities.Add(timingPoint.ToScrollVelocityInfo());
            else
                mapInfo.TimingPoints.Add(timingPoint.ToTimingPointInfo());
        }

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
