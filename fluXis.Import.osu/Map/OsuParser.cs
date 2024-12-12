using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using fluXis.Game.Utils;
using fluXis.Import.osu.Map.Components;
using fluXis.Import.osu.Map.Enums;
using osu.Framework.Graphics.Primitives;

namespace fluXis.Import.osu.Map;

public class OsuParser
{
    private List<string> general { get; } = new();
    private List<string> metadata { get; } = new();
    private List<string> difficulty { get; } = new();
    private List<string> events { get; } = new();
    private List<string> timingPoints { get; } = new();
    private List<string> hitObjects { get; } = new();

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

    public OsuMap Parse(bool eventsOnly = false)
    {
        OsuMap map = new();

        if (eventsOnly)
        {
            parseEvents(map);
            return map;
        }

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
            var splitIdx = line.IndexOf(':');
            string key = line.Substring(0, splitIdx);
            string value = line.Substring(splitIdx + 1);

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
            var splitIdx = line.IndexOf(':');
            string key = line.Substring(0, splitIdx);
            string value = line.Substring(splitIdx + 1);

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
            var splitIdx = line.IndexOf(':');
            string key = line.Substring(0, splitIdx);
            string value = line.Substring(splitIdx + 1);

            switch (key)
            {
                case "HPDrainRate":
                    map.HealthDrainRate = float.Parse(value, CultureInfo.InvariantCulture);
                    break;

                case "CircleSize":
                    map.CircleSize = float.Parse(value, CultureInfo.InvariantCulture);
                    break;

                case "OverallDifficulty":
                    map.OverallDifficulty = float.Parse(value, CultureInfo.InvariantCulture);
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
                Time = split[0].ToFloatInvariant(),
                BeatLength = split[1].ToFloatInvariant(),
                Meter = split[2].ToIntInvariant(),
                Inherited = split[6].ToIntInvariant()
            });
        }
    }

    private void parseHitObjects(OsuMap map)
    {
        foreach (var line in hitObjects)
        {
            string[] split = line.Split(',');
            string[] colonSplit = split[5].Split(':');

            var pos = new Vector2I(int.Parse(split[0]), int.Parse(split[1]));
            var startTime = split[2].ToFloatInvariant();
            var type = (OsuHitObjectType)int.Parse(split[3]);
            var sound = (OsuHitSound)int.Parse(split[4]);
            var customSound = colonSplit.LastOrDefault();

            var hit = new OsuHitObject
            {
                Position = pos,
                StartTime = startTime,
                HitSound = sound,
                CustomHitSound = customSound,
                Type = type
            };

            if (type.HasFlag(OsuHitObjectType.Hold))
                hit.EndTime = colonSplit[0].ToFloatInvariant();

            map.HitObjects.Add(hit);
        }
    }
}
