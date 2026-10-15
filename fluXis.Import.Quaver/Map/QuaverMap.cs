using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using fluXis.Import.Quaver.Map.Structs;
using fluXis.Map;
using fluXis.Map.Structures;
using fluXis.Map.Structures.Events;
using fluXis.Map.Structures.Events.Scrolling;
using fluXis.Mode.Keys.Objects;
using JetBrains.Annotations;
using osu.Framework.Platform;

namespace fluXis.Import.Quaver.Map;

[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class QuaverMap
{
    public string AudioFile { get; set; }
    public int SongPreviewTime { get; set; }
    public string BackgroundFile { get; set; }
    public string Title { get; set; }
    public string Artist { get; set; }
    public string Source { get; set; }
    public string Tags { get; set; }
    public string Creator { get; set; }
    public string DifficultyName { get; set; }

    public List<QuaverHitObjectInfo> HitObjects { get; set; } = new();
    public List<QuaverTimingPointInfo> TimingPoints { get; set; } = new();
    public List<QuaverSliderVelocityInfo> SliderVelocities { get; set; } = new();
    public Dictionary<string, QuaverTimingGroup> TimingGroups { get; set; } = new();
    public List<QuaverScrollFactor> ScrollSpeedFactors { get; set; } = new();
    public List<QuaverBookmark> Bookmarks { get; set; } = new();

    public PlayableMap ToPlayableMap(Storage storage)
    {
        int keyCount = HitObjects.Max(x => x.Lane); // ideally this should read from the Mode field but uuuh this was quicker
        var mapInfo = new PlayableMap(storage, $"{DifficultyName}[{Creator}].rhym", new("flux", $"keys/{keyCount}"))
        {
            AudioFile = AudioFile,
            BackgroundFile = BackgroundFile,
            Title = Title,
            Artist = Artist,
            Sources = new Dictionary<string, string>()
            {
                { "audio", Source },
                { "background", "" },
                { "cover", "" }
            },
            Tags = Tags.Split(','),
            Creator = Creator,
            Difficulty = DifficultyName,
            PreviewTime = SongPreviewTime,
        };

        foreach (var o in HitObjects)
        {
            // we don't support LN mines, so let's just put a bunch of mines every 1/4 beat
            if (o.Type == "Mine" && o.IsLongNote)
            {
                TimingPoint timingPoint = mapInfo.GetTimingPoint(o.StartTime);

                for (float t = o.StartTime; t < o.EndTime; t += timingPoint.MsPerBeat / 4)
                {
                    mapInfo.AddObject(new Landmine
                    {
                        Time = t,
                        Lane = o.Lane,
                        Group = o.TimingGroup
                    });

                    timingPoint = mapInfo.GetTimingPoint(o.StartTime); // in case the bpm changes during the LN mine (idk might happen on some weird maps)
                }

                // if the endtime is too far from the last mine we placed then put a mine directly at the endtime
                if (Math.Abs(mapInfo.ObjectsOfType<Landmine>().Last().Time - o.EndTime) > timingPoint.MsPerBeat / 16)
                {
                    mapInfo.AddObject(new Landmine
                    {
                        Time = o.EndTime,
                        Lane = o.Lane,
                        Group = o.TimingGroup
                    });
                }
            }
            else
            {
                if (o.Type == "Mine")
                {
                    mapInfo.AddObject(new Landmine
                    {
                        Time = o.StartTime,
                        Lane = o.Lane,
                        Group = o.TimingGroup
                    });
                }
                else if (o.IsLongNote)
                {
                    mapInfo.AddObject(new LongNote
                    {
                        Time = o.StartTime,
                        Lane = o.Lane,
                        Duration = o.EndTime - o.StartTime,
                        Group = o.TimingGroup
                    });
                }
                else
                {
                    mapInfo.AddObject(new Note
                    {
                        Time = o.StartTime,
                        Lane = o.Lane,
                        Group = o.TimingGroup
                    });
                }
            }
        }

        foreach (var t in TimingPoints)
        {
            mapInfo.AddObject(new TimingPoint
            {
                Time = t.StartTime,
                BPM = t.Bpm,
                Signature = t.TimeSignature > 0 ? t.TimeSignature : 4,
                HideLines = t.Hidden
            });
        }

        foreach (var s in SliderVelocities)
        {
            mapInfo.AddObject(new ScrollVelocity
            {
                Time = s.StartTime,
                Multiplier = s.Multiplier
            });
        }

        for (var i = 0; i < ScrollSpeedFactors.Count; i++)
        {
            var factor = ScrollSpeedFactors[i];
            var duration = 0d;
            var mult = factor.Multiplier;

            if (i + 1 < ScrollSpeedFactors.Count)
            {
                var next = ScrollSpeedFactors[i + 1];
                duration = Math.Abs(next.StartTime - factor.StartTime);
                mult = next.Multiplier;
            }

            mapInfo.AddObject(new ScrollMultiplierEvent
            {
                Time = factor.StartTime,
                Multiplier = mult,
                Duration = duration
            });
        }

        foreach (var (key, group) in TimingGroups)
        {
            foreach (var velocity in group.ScrollVelocities)
            {
                mapInfo.AddObject(new ScrollVelocity
                {
                    Time = velocity.StartTime,
                    Multiplier = velocity.Multiplier,
                    Groups = new List<string> { key }
                });
            }

            for (var i = 0; i < group.ScrollSpeedFactors.Count; i++)
            {
                var factor = group.ScrollSpeedFactors[i];
                var duration = 0d;
                var mult = factor.Multiplier;

                if (i + 1 < group.ScrollSpeedFactors.Count)
                {
                    var next = group.ScrollSpeedFactors[i + 1];
                    duration = Math.Abs(next.StartTime - factor.StartTime);
                    mult = next.Multiplier;
                }

                mapInfo.AddObject(new ScrollMultiplierEvent
                {
                    Time = factor.StartTime,
                    Multiplier = mult,
                    Duration = duration,
                    Groups = new List<string> { key }
                });
            }
        }

        return mapInfo;
    }
}
