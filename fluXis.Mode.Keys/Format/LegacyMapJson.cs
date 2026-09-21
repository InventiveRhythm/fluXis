using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Map.Structures;
using fluXis.Map.Structures.Events;
using fluXis.Online.API.Models.Maps;
using Newtonsoft.Json;

namespace fluXis.Mode.Keys.Format;

#pragma warning disable CS0612 // Type or member is obsolete
public class LegacyMapJson
{
    public string AudioFile { get; set; } = string.Empty;
    public string BackgroundFile { get; set; } = string.Empty;
    public string CoverFile { get; set; } = string.Empty;
    public string VideoFile { get; set; } = string.Empty;
    public string EffectFile { get; set; } = string.Empty;
    public string StoryboardFile { get; set; } = string.Empty;

    [JsonProperty("metadata")]
    public LegacyMapMetadata Metadata { get; set; }

    [JsonProperty("colors")]
    public LegacyMapColors Colors { get; set; } = new();

    // ReSharper disable CollectionNeverUpdated.Global
    public List<LegacyHitObject> HitObjects { get; init; }
    public List<TimingPoint> TimingPoints { get; init; }
    public List<ScrollVelocity> ScrollVelocities { get; init; }
    public List<HitSoundFade> HitSoundFades { get; set; }

    public float AccuracyDifficulty { get; set; } = 8;
    public float HealthDifficulty { get; set; } = 8;

    [JsonProperty("dual")]
    public DualMode DualMode { get; set; } = DualMode.Disabled;

    [JsonProperty("ls-v2")]
    public bool NewLaneSwitchLayout { get; set; }

    [JsonProperty("force-169")]
    public bool Force16By9 { get; set; }

    [JsonProperty("editor-time")]
    public long TimeInEditor { get; set; }

    [JsonProperty("extra-playfields")]
    public int ExtraPlayfields
    {
        get => extraPlayfields;
        set => extraPlayfields = Math.Clamp(value, 0, 9);
    }

    [JsonIgnore]
    private int extraPlayfields;

    [JsonProperty("visualization-enabled")]
    public bool EnableVisualization { get; set; }

    #region Server-Side Stuff

    [JsonIgnore]
    public string RawContent { get; set; } = "";

    [JsonIgnore]
    public string FileName { get; set; } = "";

    [JsonIgnore]
    public int KeyCount => HitObjects.Max(x => x.Lane);

    #endregion

    public LegacyMapJson(LegacyMapMetadata metadata)
        : this()
    {
        Metadata = metadata;
    }

    public LegacyMapJson()
    {
        Metadata = new LegacyMapMetadata();
        HitObjects = new List<LegacyHitObject>();
        TimingPoints = new List<TimingPoint> { new() { BPM = 120, Time = 0, Signature = 4 } }; // Add default timing point to avoid issues
        ScrollVelocities = new List<ScrollVelocity>();
        HitSoundFades = new List<HitSoundFade>();
    }

    /*public LegacyMapEvents GetMapEvents() => GetMapEvents<LegacyMapEvents>();*/

    /*public virtual T GetMapEvents<T>()
        where T : LegacyMapEvents, new()
    {
        var events = new T();

        if (RealmEntry == null)
            return events;

        var effectFile = RealmEntry.MapSet.GetPathForFile(EffectFile);

        if (string.IsNullOrEmpty(effectFile))
            return events;

        if (!File.Exists(MapFiles.GetFullPath(effectFile)))
            return events;

        var content = File.ReadAllText(MapFiles.GetFullPath(effectFile));
        EffectHash = MapUtils.GetHash(content);
        return LegacyMapEvents.Load<T>(content);
    }

    [CanBeNull]
    public virtual Storyboard GetStoryboard()
    {
        var file = RealmEntry?.MapSet.GetPathForFile(StoryboardFile);

        if (string.IsNullOrEmpty(file))
            return null;

        var path = MapFiles.GetFullPath(file);

        if (string.IsNullOrEmpty(path) || !File.Exists(path))
            return null;

        var json = File.ReadAllText(path);
        StoryboardHash = MapUtils.GetHash(json);

        var sb = json.Deserialize<Storyboard>();
        sb.Update();
        return sb;
    }

    [CanBeNull]
    public virtual DrawableStoryboard CreateDrawableStoryboard()
    {
        var sb = GetStoryboard();

        if (sb == null)
            return null;

        var folderName = RealmEntry?.MapSet.ID.ToString();

        if (string.IsNullOrEmpty(folderName))
            return null;

        var path = MapFiles.GetFullPath(folderName);

        if (!Directory.Exists(path))
            return null;

        return new DrawableStoryboard(null, sb, MapFiles.GetFullPath(RealmEntry!.MapSet.ID.ToString()));
    }

    [CanBeNull]
    public virtual Stream GetVideoStream()
    {
        var file = RealmEntry?.MapSet.GetPathForFile(VideoFile);

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
    }*/
}
#pragma warning restore CS0612 // Type or member is obsolete
