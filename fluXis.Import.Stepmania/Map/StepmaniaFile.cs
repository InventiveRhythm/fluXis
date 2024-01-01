using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using fluXis.Game.Map;
using fluXis.Game.Map.Structures;
using fluXis.Import.Stepmania.Map.Components;

namespace fluXis.Import.Stepmania.Map;

public class StepmaniaFile
{
    public string Title { get; set; }
    public string Artist { get; set; }
    public string Credit { get; set; }
    public string Background { get; set; }
    public string Music { get; set; }
    public float Offset { get; set; }
    public float SampleStart { get; set; }

    public List<StepBpm> Bpms { get; set; } = new();
    public List<StepStop> Stops { get; set; } = new();
    public List<StepChart> Charts { get; set; } = new();

    public void Parse(string data)
    {
        var lines = data.Split('\n');

        StepChart chart = null;
        bool bpms = false;
        bool stops = false;

        foreach (var line in lines)
        {
            var trimmed = line.Trim();

            if (trimmed.StartsWith("#") && trimmed.Contains(':'))
            {
                var split = trimmed.Split(':');
                var key = split[0].Trim().Replace("#", "");
                var value = split[1].Replace(";", "").Replace(":", "");

                switch (key)
                {
                    case "TITLE":
                        Title = value;
                        break;

                    case "ARTIST":
                        Artist = value;
                        break;

                    case "CREDIT":
                        Credit = value;
                        break;

                    case "BACKGROUND":
                        Background = value;
                        break;

                    case "MUSIC":
                        Music = value;
                        break;

                    case "OFFSET":
                        Offset = float.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
                        break;

                    case "SAMPLESTART":
                        SampleStart = float.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
                        break;

                    case "BPMS":
                        bpms = true;
                        Bpms = StepBpm.Parse(value);
                        if (line.Contains(';')) bpms = false;
                        break;

                    case "STOPS":
                        stops = true;
                        Stops = StepStop.Parse(value);
                        if (line.Contains(';')) stops = false;
                        break;

                    case "NOTES":
                        chart = new StepChart();
                        Charts.Add(chart);
                        break;
                }
            }
            else if (bpms)
            {
                Bpms.AddRange(StepBpm.Parse(line));
                if (line.Contains(';')) bpms = false;
            }
            else if (stops)
            {
                Stops.AddRange(StepStop.Parse(line));
                if (line.Contains(';')) stops = false;
            }
            else if (trimmed.StartsWith("//")) { } // Comment
            else if (chart is { GrooveRadar: null })
            {
                var value = trimmed.Replace(":", "");

                if (chart.Type == null)
                    chart.Type = value;
                else if (chart.Description == null)
                    chart.Description = value;
                else if (chart.Difficulty == null)
                    chart.Difficulty = value;
                else if (chart.Meter == null)
                    chart.Meter = value;
                else if (chart.GrooveRadar == null)
                    chart.GrooveRadar = value;
            }
            else if (chart is { GrooveRadar: not null } && !string.IsNullOrEmpty(trimmed))
            {
                if (trimmed.StartsWith(","))
                {
                    chart.Measures.Add(new StepMeasure(new List<List<StepNote>>()));
                    continue;
                }

                if (trimmed.StartsWith(";"))
                {
                    chart = null;
                    continue;
                }

                chart.Measures.Last().Notes.Add(StepMeasure.Parse(trimmed));
            }
        }
    }

    public List<MapInfo> ToMapInfos()
    {
        var maps = new List<MapInfo>();

        foreach (var chart in Charts)
        {
            var time = -Offset * 1000;

            var map = new MapInfo(null)
            {
                AudioFile = Music,
                BackgroundFile = Background,
                VideoFile = "",
                EffectFile = "",
                Metadata = new MapMetadata
                {
                    Title = Title,
                    Artist = Artist,
                    Mapper = Credit,
                    Difficulty = chart.Difficulty,
                    Source = "",
                    Tags = "",
                    PreviewTime = (int)(SampleStart * 1000)
                },
                TimingPoints = new List<TimingPoint>()
            };

            var beats = 0f;
            var bpms = Bpms.ToList();
            var stops = Stops.ToList();

            foreach (var measure in chart.Measures)
            {
                var increment = 4f / measure.Notes.Count;

                foreach (var row in measure.Notes)
                {
                    if (bpms.Count != 0 && beats >= bpms.First().Beat)
                    {
                        map.TimingPoints.Add(new TimingPoint
                        {
                            Time = time,
                            Signature = 4,
                            BPM = bpms.First().BPM
                        });

                        bpms.Remove(bpms.First());
                    }

                    for (int i = 0; i < row.Count; i++)
                    {
                        switch (row[i])
                        {
                            case StepNote.None:
                                break;

                            case StepNote.Normal:
                                map.HitObjects.Add(new HitObject
                                {
                                    Time = (int)Math.Round(time, MidpointRounding.AwayFromZero),
                                    Lane = i + 1
                                });
                                break;

                            case StepNote.Head:
                                map.HitObjects.Add(new HitObject
                                {
                                    Time = (int)Math.Round(time, MidpointRounding.AwayFromZero),
                                    HoldTime = int.MinValue,
                                    Lane = i + 1
                                });
                                break;

                            case StepNote.Tail:
                                var note = map.HitObjects.FindLast(x => x.Lane == i + 1);

                                if (note is StepRollNote roll)
                                {
                                    var timeDiff = time - roll.Time;
                                    var distance = map.GetTimingPoint(time).MsPerBeat / 2;
                                    var notes = (int)(timeDiff / distance);

                                    for (int j = 0; j < notes; j++)
                                    {
                                        map.HitObjects.Add(new HitObject
                                        {
                                            Time = (int)Math.Round(roll.Time + distance * (j + 1), MidpointRounding.AwayFromZero),
                                            Lane = i + 1
                                        });
                                    }
                                }
                                else if (note != null)
                                    note.EndTime = (int)Math.Round(time, MidpointRounding.AwayFromZero);

                                break;

                            case StepNote.Roll:
                                map.HitObjects.Add(new StepRollNote()
                                {
                                    Time = (int)Math.Round(time, MidpointRounding.AwayFromZero),
                                    Lane = i + 1
                                });
                                break;

                            case StepNote.Mine:
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    time += map.GetTimingPoint(time).MsPerBeat * 4 / measure.Notes.Count;
                    beats += increment * 4;

                    if (stops.Count == 0 || !(beats > stops.First().Beat)) continue;

                    time += stops.First().Seconds * 1000;
                    stops.Remove(stops.First());
                }
            }

            maps.Add(map);
        }

        return maps;
    }
}
