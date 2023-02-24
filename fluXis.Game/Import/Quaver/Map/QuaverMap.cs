using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using fluXis.Game.Import.Quaver.Map.Structs;
using fluXis.Game.Map;
using osu.Framework.Logging;

namespace fluXis.Game.Import.Quaver.Map;

[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
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
            Difficulty = DifficultyName
        };

        var mapInfo = new MapInfo(metadata)
        {
            ID = "",
            MD5 = "",
            AudioFile = AudioFile,
            BackgroundFile = BackgroundFile,
            HitObjects = new List<HitObjectInfo>(),
            TimingPoints = new List<TimingPointInfo>(),
            ScrollVelocities = new List<ScrollVelocityInfo>(),
            Events = new List<EventInfo>()
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

        foreach (var b in Bookmarks)
        {
            var split = b.Note.ToLower().Split(':');

            if (split.Length != 2)
                continue;

            if (split[0] == "laneswitch")
            {
                if (int.TryParse(split[1], out var lane))
                {
                    mapInfo.Events.Add(new EventInfo
                    {
                        Time = b.StartTime,
                        Type = "laneswitch",
                        Value = lane
                    });
                }
                else
                {
                    Logger.Log("Invalid lane switch event: " + b.Note, LoggingTarget.Runtime, LogLevel.Error);
                }
            }
        }

        return mapInfo;
    }
}
