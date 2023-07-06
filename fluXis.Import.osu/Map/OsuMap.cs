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
    // dont need this

    // [Events]
    public List<OsuEvent> Events { get; init; }

    // [TimingPoints]
    public List<OsuTimingPoint> TimingPoints { get; init; }

    // [Colours]
    // dont need this

    // [HitObjects]
    public List<OsuHitObject> HitObjects { get; init; }

    public OsuMap()
    {
        Events = new List<OsuEvent>();
        TimingPoints = new List<OsuTimingPoint>();
        HitObjects = new List<OsuHitObject>();
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
            KeyCount = 4,
            InitialKeyCount = 4
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
