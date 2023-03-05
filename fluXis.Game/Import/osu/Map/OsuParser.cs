using System.Collections.Generic;
using System.Globalization;
using fluXis.Game.Import.osu.Map.Components;
using fluXis.Game.Import.osu.Map.Enums;

namespace fluXis.Game.Import.osu.Map;

public class OsuParser
{
    public List<string> General { get; set; }
    public List<string> Metadata { get; set; }
    public List<string> Events { get; set; }
    public List<string> TimingPoints { get; set; }
    public List<string> HitObjects { get; set; }

    public OsuParser()
    {
        General = new List<string>();
        Metadata = new List<string>();
        Events = new List<string>();
        TimingPoints = new List<string>();
        HitObjects = new List<string>();
    }

    public void AddLine(string line, OsuFileSection section)
    {
        switch (section)
        {
            case OsuFileSection.General:
                General.Add(line);
                break;

            case OsuFileSection.Metadata:
                Metadata.Add(line);
                break;

            case OsuFileSection.Events:
                Events.Add(line);
                break;

            case OsuFileSection.TimingPoints:
                TimingPoints.Add(line);
                break;

            case OsuFileSection.HitObjects:
                HitObjects.Add(line);
                break;
        }
    }

    public OsuMap Parse()
    {
        OsuMap map = new();

        parseGeneral(map);
        parseMetadata(map);
        parseEvents(map);
        parseTimingPoints(map);
        parseHitObjects(map);

        return map;
    }

    private void parseGeneral(OsuMap map)
    {
        foreach (var line in General)
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
        foreach (var line in Metadata)
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

    private void parseEvents(OsuMap map)
    {
        foreach (var line in Events)
        {
            string[] split = line.Split(',');

            OsuEvent @event = new OsuEvent
            {
                EventType = split[0],
                StartTime = split[1],
                Parameter = split[2]
            };

            map.Events.Add(@event);
        }
    }

    private void parseTimingPoints(OsuMap map)
    {
        foreach (var line in TimingPoints)
        {
            string[] split = line.Split(',');

            map.TimingPoints.Add(new OsuTimingPoint
            {
                Time = int.Parse(split[0]),
                BeatLength = decimal.Parse(split[1], CultureInfo.InvariantCulture),
                Meter = int.Parse(split[2]),
                Inherited = int.Parse(split[6])
            });
        }
    }

    private void parseHitObjects(OsuMap map)
    {
        foreach (var line in HitObjects)
        {
            string[] split = line.Split(',');

            map.HitObjects.Add(new OsuHitObject
            {
                X = int.Parse(split[0]),
                StartTime = int.Parse(split[2]),
                EndTime = int.Parse(split[5].Split(":")[0])
            });
        }
    }
}
