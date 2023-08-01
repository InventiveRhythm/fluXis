using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using fluXis.Game.Map;
using fluXis.Import.Quaver.Map.Structs;
using JetBrains.Annotations;
using osu.Framework.Logging;

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

    public List<QuaverHitObjectInfo> HitObjects { get; set; }
    public List<QuaverTimingPointInfo> TimingPoints { get; set; }
    public List<QuaverSliderVelocityInfo> SliderVelocities { get; set; }
    public List<QuaverBookmark> Bookmarks { get; set; }

    public MapInfo ToMapInfo()
    {
        var metadata = new MapMetadata
        {
            Title = Title,
            Artist = Artist,
            Source = Source,
            Tags = Tags,
            Mapper = Creator,
            Difficulty = DifficultyName,
            PreviewTime = SongPreviewTime
        };

        var mapInfo = new QuaverMapInfo(metadata)
        {
            Map = this,
            AudioFile = AudioFile,
            BackgroundFile = BackgroundFile,
            HitObjects = new List<HitObjectInfo>(),
            TimingPoints = new List<TimingPointInfo>(),
            ScrollVelocities = new List<ScrollVelocityInfo>()
        };

        foreach (var o in HitObjects)
        {
            mapInfo.HitObjects.Add(new HitObjectInfo
            {
                Time = o.StartTime,
                Lane = o.Lane,
                HoldTime = o.IsLongNote ? o.EndTime - o.StartTime : 0
            });
        }

        foreach (var t in TimingPoints)
        {
            mapInfo.TimingPoints.Add(new TimingPointInfo
            {
                Time = t.StartTime,
                BPM = t.Bpm,
                Signature = t.TimeSignature > 0 ? t.TimeSignature : 4,
                HideLines = t.Hidden
            });
        }

        foreach (var s in SliderVelocities)
        {
            mapInfo.ScrollVelocities.Add(new ScrollVelocityInfo
            {
                Time = s.StartTime,
                Multiplier = s.Multiplier
            });
        }

        return mapInfo;
    }

    public MapEvents GetEffects()
    {
        string effectFile = "";

        if (Bookmarks != null)
        {
            foreach (var b in Bookmarks)
            {
                var split = b.Note.ToLower().Split(':');
                var split2 = b.Note.Split(':');

                if (split[0] == "laneswitch" && split.Length >= 2)
                {
                    if (int.TryParse(split[1], out var lane))
                    {
                        if (split.Length > 2 && int.TryParse(split[2], out var speed))
                            effectFile += $"LaneSwitch({b.StartTime},{lane},{speed}){Environment.NewLine}";
                        else
                            effectFile += $"LaneSwitch({b.StartTime},{lane}){Environment.NewLine}";
                    }
                    else
                        Logger.Log("Invalid lane switch event: " + b.Note, LoggingTarget.Runtime, LogLevel.Error);
                }

                if (split[0] == "flash" && split.Length == 8)
                    effectFile += $"Flash({b.StartTime},{split[1]},{split[2]},{split2[3]},{split[4]},{split[5]},{split[6]},{split[7]}){Environment.NewLine}";

                if (split[0] == "pulse" && split.Length == 1)
                    effectFile += $"Pulse({b.StartTime}){Environment.NewLine}";

                if (split[0] == "playfieldmove" && split.Length >= 2)
                    effectFile += $"PlayfieldMove({b.StartTime},{split[1]},{(split.Length >= 3 ? split[2] : 0)},{(split.Length >= 4 ? split2[3] : "None")}){Environment.NewLine}";
            }
        }

        return new MapEvents().Load(effectFile);
    }
}
