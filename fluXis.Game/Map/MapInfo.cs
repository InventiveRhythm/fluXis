using System.Collections.Generic;
using System.IO;
using System.Linq;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map.Structures;
using fluXis.Game.Storyboards;
using fluXis.Game.Storyboards.Drawables;
using fluXis.Shared.Utils;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace fluXis.Game.Map;

public class MapInfo
{
    public string AudioFile { get; set; } = string.Empty;
    public string BackgroundFile { get; set; } = string.Empty;
    public string CoverFile { get; set; } = string.Empty;
    public string VideoFile { get; set; } = string.Empty;
    public string EffectFile { get; set; } = string.Empty;
    public string StoryboardFile { get; set; } = string.Empty;
    public MapMetadata Metadata { get; set; }
    public List<HitObject> HitObjects { get; set; }
    public List<TimingPoint> TimingPoints { get; set; }
    public List<ScrollVelocity> ScrollVelocities { get; set; }
    public List<HitSoundFade> HitSoundFades { get; set; }

    public float AccuracyDifficulty { get; set; } = 8;

    [JsonIgnore]
    public float StartTime => HitObjects[0].Time;

    [JsonIgnore]
    public float EndTime => HitObjects[^1].EndTime;

    [JsonIgnore]
    public int MaxCombo
    {
        get
        {
            int maxCombo = 0;

            foreach (var hitObject in HitObjects)
            {
                maxCombo++;
                if (hitObject.LongNote)
                    maxCombo++;
            }

            return maxCombo;
        }
    }

    [JsonIgnore]
    public int InitialKeyCount { get; set; }

    [CanBeNull]
    [JsonIgnore]
    public RealmMap Map { get; set; }

    [JsonIgnore]
    public string Hash { get; set; }

    public MapInfo(MapMetadata metadata)
        : this()
    {
        Metadata = metadata;
    }

    public MapInfo()
    {
        Metadata = new MapMetadata();
        HitObjects = new List<HitObject>();
        TimingPoints = new List<TimingPoint> { new() { BPM = 120, Time = 0, Signature = 4 } }; // Add default timing point to avoid issues
        ScrollVelocities = new List<ScrollVelocity>();
        HitSoundFades = new List<HitSoundFade>();
    }

    public bool Validate(out string issue)
    {
        if (HitObjects.Count == 0)
        {
            issue = "Map has no hit objects";
            return false;
        }

        if (TimingPoints.Count == 0)
        {
            issue = "Map has no timing points";
            return false;
        }

        foreach (var timingPoint in TimingPoints)
        {
            if (timingPoint.BPM <= 0)
            {
                issue = "A timing point has an invalid BPM";
                return false;
            }

            if (timingPoint.Signature <= 0)
            {
                issue = "A timing point has an invalid signature";
                return false;
            }
        }

        if (HitObjects.Any(hitObject => hitObject.Lane < 1))
        {
            issue = "Map has an invalid key count";
            return false;
        }

        issue = string.Empty;
        return true;
    }

    public void Sort()
    {
        HitObjects.Sort((a, b) => a.Time == b.Time ? a.Lane.CompareTo(b.Lane) : a.Time.CompareTo(b.Time));
        TimingPoints.Sort((a, b) => a.Time.CompareTo(b.Time));
        ScrollVelocities?.Sort((a, b) => a.Time.CompareTo(b.Time));
        HitSoundFades?.Sort((a, b) => a.Time.CompareTo(b.Time));
    }

    public MapEvents GetMapEvents() => GetMapEvents<MapEvents>();

    public virtual T GetMapEvents<T>()
        where T : MapEvents, new()
    {
        var events = new T();

        if (Map == null)
            return events;

        var effectFile = Map.MapSet.GetPathForFile(EffectFile);

        if (string.IsNullOrEmpty(effectFile))
            return events;

        if (!File.Exists(MapFiles.GetFullPath(effectFile)))
            return events;

        var content = File.ReadAllText(MapFiles.GetFullPath(effectFile));
        return MapEvents.Load<T>(content);
    }

    public virtual Storyboard GetStoryboard()
    {
        var file = Map?.MapSet.GetPathForFile(StoryboardFile);

        if (string.IsNullOrEmpty(file))
            return new Storyboard();

        var path = MapFiles.GetFullPath(file);

        if (string.IsNullOrEmpty(path) || !File.Exists(path))
            return new Storyboard();

        var json = File.ReadAllText(path);
        return json.Deserialize<Storyboard>();
    }

    public virtual DrawableStoryboard CreateDrawableStoryboard()
    {
        var sb = GetStoryboard();
        var folderName = Map?.MapSet.ID.ToString();

        if (string.IsNullOrEmpty(folderName))
            return new DrawableStoryboard(sb, ".");

        var path = MapFiles.GetFullPath(folderName);

        if (!Directory.Exists(path))
            return new DrawableStoryboard(sb, ".");

        return new DrawableStoryboard(sb, MapFiles.GetFullPath(Map!.MapSet.ID.ToString()));
    }

    public TimingPoint GetTimingPoint(double time)
    {
        if (TimingPoints.Count == 0)
            return new TimingPoint { BPM = 60, Time = 0 };

        TimingPoint timingPoint = null;

        foreach (var tp in TimingPoints)
        {
            if (tp.Time > time)
                break;

            timingPoint = tp;
        }

        return timingPoint ?? TimingPoints[0];
    }

    public MapInfo Clone()
    {
        return new MapInfo(Metadata)
        {
            AudioFile = AudioFile,
            BackgroundFile = BackgroundFile,
            CoverFile = CoverFile,
            VideoFile = VideoFile,
            EffectFile = EffectFile,
            HitObjects = HitObjects,
            TimingPoints = TimingPoints,
            ScrollVelocities = ScrollVelocities,
            HitSoundFades = HitSoundFades,
            InitialKeyCount = InitialKeyCount,
            AccuracyDifficulty = AccuracyDifficulty,
            Map = Map
        };
    }
}
