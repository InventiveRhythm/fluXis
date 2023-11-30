using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using fluXis.Import.osu.Map.Components;
using fluXis.Import.osu.Map.Enums;

namespace fluXis.Import.osu.Map;

public class OsuParser
{
    private List<string> general { get; }
    private List<string> metadata { get; }
    private List<string> difficulty { get; }
    private List<string> events { get; }
    private List<string> timingPoints { get; }
    private List<string> hitObjects { get; }

    public OsuParser()
    {
        general = new List<string>();
        metadata = new List<string>();
        difficulty = new List<string>();
        events = new List<string>();
        timingPoints = new List<string>();
        hitObjects = new List<string>();
    }

    public void AddLine(string line, OsuFileSection section)
    {
        switch (section)
        {
            case OsuFileSection.General:
                general.Add(line);
                break;

            case OsuFileSection.Metadata:
                metadata.Add(line);
                break;

            case OsuFileSection.Difficulty:
                difficulty.Add(line);
                break;

            case OsuFileSection.Events:
                events.Add(line);
                break;

            case OsuFileSection.TimingPoints:
                timingPoints.Add(line);
                break;

            case OsuFileSection.HitObjects:
                hitObjects.Add(line);
                break;
        }
    }

    public OsuMap Parse()
    {
        OsuMap map = new();

        parseGeneral(map);
        parseMetadata(map);
        parseDifficulty(map);
        parseEvents(map);
        parseTimingPoints(map);
        parseHitObjects(map);

        return map;
    }

    private void parseGeneral(OsuMap map)
    {
        foreach (var line in general)
        {
            string[] split = line.Split(':');
            string key = split[0];
            string value = split[1];

            switch (key)
            {
                case "AudioFilename":
                    map.AudioFilename = value;
                    break;

                case "PreviewTime":
                    map.PreviewTime = int.Parse(value);
                    break;

                case "Mode":
                    map.Mode = int.Parse(value);
                    break;
            }
        }
    }

    private void parseMetadata(OsuMap map)
    {
        foreach (var line in metadata)
        {
            string[] split = line.Split(':');
            string key = split[0];
            string value = split[1];

            switch (key)
            {
                case "Title":
                    map.Title = value;
                    break;

                case "Artist":
                    map.Artist = value;
                    break;

                case "Creator":
                    map.Creator = value;
                    break;

                case "Version":
                    map.Version = value;
                    break;

                case "Source":
                    map.Source = value;
                    break;

                case "Tags":
                    map.Tags = value;
                    break;
            }
        }
    }

    private void parseDifficulty(OsuMap map)
    {
        foreach (var line in difficulty)
        {
            string[] split = line.Split(':');
            string key = split[0];
            string value = split[1];

            switch (key)
            {
                case "CircleSize":
                    map.CircleSize = float.Parse(value, CultureInfo.InvariantCulture);
                    break;
            }
        }
    }

    private void parseEvents(OsuMap map)
    {
        foreach (var line in events)
        {
            string[] split = line.Split(',');

            OsuEvent @event = new OsuEvent
            {
                EventType = split[0],
                Parameter = split[2]
            };

            map.Events.Add(@event);
        }
    }

    private void parseTimingPoints(OsuMap map)
    {
        foreach (var line in timingPoints)
        {
            string[] split = line.Split(',');

            map.TimingPoints.Add(new OsuTimingPoint
            {
                Time = float.Parse(split[0]),
                BeatLength = float.Parse(split[1], CultureInfo.InvariantCulture),
                Meter = int.Parse(split[2]),
                Inherited = int.Parse(split[6])
            });
        }
    }

    private void parseHitObjects(OsuMap map)
    {
        foreach (var line in hitObjects)
        {
            string[] split = line.Split(',');
            string[] colonSplit = split[5].Split(":");

            map.HitObjects.Add(new OsuHitObject
            {
                X = int.Parse(split[0]),
                StartTime = int.Parse(split[2]),
                EndTime = int.Parse(colonSplit[0]),
                HitSound = colonSplit.LastOrDefault() ?? ""
            });
        }
    }
}
