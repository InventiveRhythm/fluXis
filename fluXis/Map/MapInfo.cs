using System.Collections.Generic;
using System.IO;
using System.Linq;
using fluXis.Database;
using fluXis.Database.Maps;
using fluXis.Map.Structures;
using fluXis.Mods;
using fluXis.Online.API.Models.Maps;
using fluXis.Storyboards;
using fluXis.Storyboards.Drawables;
using fluXis.Utils;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace fluXis.Map;

public class MapInfo
{
    public static int MinKeymode { get; set; } = 1;
    public static int MaxKeymode { get; set; } = -1; // allow anything

    public string AudioFile { get; set; } = string.Empty;
    public string BackgroundFile { get; set; } = string.Empty;
    public string CoverFile { get; set; } = string.Empty;
    public string VideoFile { get; set; } = string.Empty;
    public string EffectFile { get; set; } = string.Empty;
    public string StoryboardFile { get; set; } = string.Empty;

    [JsonProperty("metadata")]
    public MapMetadata Metadata { get; set; }

    [JsonProperty("colors")]
    public MapColors Colors { get; set; } = new();

    public List<HitObject> HitObjects { get; set; }
    public List<TimingPoint> TimingPoints { get; set; }
    public List<ScrollVelocity> ScrollVelocities { get; set; }
    public List<HitSoundFade> HitSoundFades { get; set; }

    public float AccuracyDifficulty { get; set; } = 8;
    public float HealthDifficulty { get; set; } = 8;

    [JsonProperty("dual")]
    public DualMode DualMode { get; set; } = DualMode.Disabled;

    [JsonProperty("ls-v2")]
    public bool NewLaneSwitchLayout { get; set; }

    [JsonProperty("editor-time")]
    public long TimeInEditor { get; set; }

    [JsonIgnore]
    public bool IsDual => DualMode > DualMode.Disabled;

    [JsonIgnore]
    public double StartTime => HitObjects[0].Time;

    [JsonIgnore]
    public double EndTime => HitObjects[^1].EndTime;

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

    [JsonIgnore]
    public string EffectHash { get; private set; }

    [JsonIgnore]
    public string StoryboardHash { get; private set; }

    #region Server-Side Stuff

    [JsonIgnore]
    public string RawContent { get; set; } = "";

    [JsonIgnore]
    public string FileName { get; set; } = "";

    [JsonIgnore]
    public int KeyCount => HitObjects.Max(x => x.Lane);

    #endregion

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
            issue = "Map has no hit objects.";
            return false;
        }

        if (TimingPoints.Count == 0)
        {
            issue = "Map has no timing points.";
            return false;
        }

        foreach (var timingPoint in TimingPoints)
        {
            if (timingPoint.BPM <= 0)
            {
                issue = "A timing point has an invalid BPM.";
                return false;
            }

            if (timingPoint.Signature <= 0)
            {
                issue = "A timing point has an invalid signature.";
                return false;
            }
        }

        if (HitObjects.Any(hitObject => hitObject.Lane < 1))
        {
            issue = "A hit object in this map is in a lane below 1.";
            return false;
        }

        var mode = HitObjects.MaxBy(x => x.Lane).Lane;

        if (mode < MinKeymode || (MaxKeymode > 0 && mode > MaxKeymode))
        {
            issue = "Map has an invalid keymode.";
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

    public MapEvents GetMapEvents(List<IMod> mods)
    {
        var events = GetMapEvents<MapEvents>();

        foreach (var mod in mods.OfType<IApplicableToEvents>())
            mod.Apply(events);

        return events;
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
        EffectHash = MapUtils.GetHash(content);
        return MapEvents.Load<T>(content);
    }

    [CanBeNull]
    public virtual Storyboard GetStoryboard()
    {
        var file = Map?.MapSet.GetPathForFile(StoryboardFile);

        if (string.IsNullOrEmpty(file))
            return null;

        var path = MapFiles.GetFullPath(file);

        if (string.IsNullOrEmpty(path) || !File.Exists(path))
            return null;

        var json = File.ReadAllText(path);
        StoryboardHash = MapUtils.GetHash(json);
        return json.Deserialize<Storyboard>();
    }

    [CanBeNull]
    public virtual DrawableStoryboard CreateDrawableStoryboard()
    {
        var sb = GetStoryboard();

        if (sb == null)
            return null;

        var folderName = Map?.MapSet.ID.ToString();

        if (string.IsNullOrEmpty(folderName))
            return null;

        var path = MapFiles.GetFullPath(folderName);

        if (!Directory.Exists(path))
            return null;

        return new DrawableStoryboard(sb, MapFiles.GetFullPath(Map!.MapSet.ID.ToString()));
    }

    public virtual Stream GetVideoStream()
    {
        var file = Map?.MapSet.GetPathForFile(VideoFile);

        if (string.IsNullOrEmpty(file))
            return null;

        var path = MapFiles.GetFullPath(file);

        if (string.IsNullOrEmpty(path) || !File.Exists(path))
            return null;

        return File.OpenRead(path);
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

    public override string ToString() => $"{Hash} - {EffectHash} - {StoryboardHash}";
}
