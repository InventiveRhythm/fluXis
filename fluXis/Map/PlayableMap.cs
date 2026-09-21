using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Database.Maps;
using fluXis.Map.Structures;
using fluXis.Map.Structures.Bases;
using fluXis.Online.API.Models.Maps;
using fluXis.Storyboards;
using fluXis.Utils;
using osu.Framework.Caching;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Extensions.ListExtensions;
using osu.Framework.Lists;
using osu.Framework.Platform;
using osuTK.Graphics;
using rhym;

namespace fluXis.Map;

#nullable enable

public class PlayableMap(Storage storage, string filename, ResourceLocation mode)
{
    public Storage Storage => storage;
    public string FileName => filename;
    public ResourceLocation Mode => mode;

    public long OnlineID { get; set; }
    public float Rating { get; set; }
    public RealmMapUserSettings Settings { get; set; } = new();

    #region Assets

    public string AudioFile { get; set; } = string.Empty;
    public string BackgroundFile { get; set; } = string.Empty;
    public string CoverFile { get; set; } = string.Empty;
    public string VideoFile { get; set; } = string.Empty;
    public Dictionary<string, string> ImportPaths { get; set; } = [];

    #endregion

    #region Editor

    public long TimeInEditor { get; set; }

    #endregion

    #region Hashes

    public string ChartHash { get; set; } = string.Empty;
    public string EffectHash { get; set; } = string.Empty;
    public string StoryboardHash { get; set; } = string.Empty;

    #endregion

    #region Metadata

    public string Title { get; set; } = string.Empty;
    public string TitleRomanized { get; set; } = string.Empty;

    public string Artist { get; set; } = string.Empty;
    public string ArtistRomanized { get; set; } = string.Empty;

    public string Difficulty { get; set; } = string.Empty;
    public string Creator { get; set; } = string.Empty;
    public int PreviewTime { get; set; }
    public Dictionary<string, Color4> Colors { get; set; } = [];
    public Dictionary<string, string> Sources { get; set; } = [];
    public string[] Tags { get; set; } = [];

    #endregion

    #region Objects

    public double StartTime => ObjectsOfType<HitObject>().FirstOrDefault()?.Time ?? 0;
    public double EndTime => ObjectsOfType<HitObject>().LastOrDefault()?.GetEndTime() ?? 1000;

    public int MaxCombo
    {
        get
        {
            if (maxCombo.IsValid)
                return maxCombo.Value;

            int c = 0;

            foreach (var hitObject in ObjectsOfType<HitObject>())
            {
                c += hitObject.ComboContribution;
            }

            return maxCombo.Value = c;
        }
    }

    private Cached<int> maxCombo { get; } = new();

    public SlimReadOnlyListWrapper<ITimedObject> Objects => objects.AsSlimReadOnly();
    private readonly List<ITimedObject> objects = [];
    private readonly Dictionary<Type, List<object>> lut = [];

    public void AddObject(ITimedObject obj)
    {
        if (obj is HitObject)
            maxCombo.Invalidate();

        objects.Add(obj);
    }

    public void AddObjects(IEnumerable<ITimedObject> objs) => objs.ForEach(AddObject);

    public void RemoveObject(ITimedObject obj)
    {
        if (obj is HitObject)
            maxCombo.Invalidate();

        objects.Remove(obj);
    }

    public void RemoveObjects(IEnumerable<ITimedObject> objs) => objs.ForEach(RemoveObject);

    public void Sort() => objects.Sort((a, b) =>
    {
        var result = a.Time.CompareTo(b.Time);
        if (result != 0) return result;

        result = a.Lane.CompareTo(b.Lane);
        return result != 0 ? result : a.GetHashCode().CompareTo(b.GetHashCode());
    });

    // TODO: optimize this with a type->list lookup table
    public T[] ObjectsOfType<T>() where T : ITimedObject => [.. objects.OfType<T>()];

    public ITimedObject[] ObjectsOfType(Type type)
    {
        // todo: this is wrong (kind of)
        return [.. objects.Where(x => x.GetType().IsAssignableTo(type))];
    }

    public TimingPoint GetTimingPoint(double time)
    {
        var points = ObjectsOfType<TimingPoint>();

        if (points.Length == 0)
            return new TimingPoint { BPM = 60, Time = 0 };

        TimingPoint? timingPoint = null;

        foreach (var tp in points)
        {
            if (tp.Time > time)
                break;

            timingPoint = tp;
        }

        return timingPoint ?? points[0];
    }

    #endregion

    #region Properties

    public float AccuracyDifficulty { get; set; } = 8;
    public float HealthDifficulty { get; set; } = 8;

    public DualMode Dual { get; set; }
    public bool IsDual => Dual > DualMode.Disabled;
    public bool IsDualSplit => Dual == DualMode.Separate;

    public bool AudioVisualizations { get; set; }
    public int ExtraPlayfields { get; set; }
    public bool ForceAspect { get; set; }
    public bool LegacyLaneSwitchLayout { get; set; }

    #endregion

    #region Storyboard

    public Storyboard? Storyboard { get; set; }

    #endregion

    public bool Validate(out string issue)
    {
        var hits = ObjectsOfType<HitObject>();
        var tps = ObjectsOfType<TimingPoint>();

        if (hits.Length == 0)
        {
            issue = "Map has no hit objects.";
            return false;
        }

        if (tps.Length == 0)
        {
            issue = "Map has no timing points.";
            return false;
        }

        foreach (var timingPoint in tps)
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

        if (hits.Any(hitObject => hitObject.Lane < 1))
        {
            issue = "A hit object in this map is in a lane below 1.";
            return false;
        }

        issue = string.Empty;
        return true;
    }

    public void SaveIntoRealmMap(RealmMap target)
    {
        target.Metadata ??= new RealmMapMetadata();
        target.Metadata.Title = Title;
        target.Metadata.TitleRomanized = TitleRomanized;
        target.Metadata.Artist = Artist;
        target.Metadata.ArtistRomanized = ArtistRomanized;
        target.Metadata.Mapper = Creator;
        target.Metadata.Source = Sources.GetValueOrDefault("$audio", "");
        target.Metadata.Tags = string.Join(", ", Tags);
        target.Metadata.Background = BackgroundFile;
        target.Metadata.Audio = AudioFile;
        target.Metadata.PreviewTime = PreviewTime;
        target.Metadata.ColorHex = Colors.TryGetValue("$accent", out var c) ? c.ToHex() : "";

        target.Difficulty = Difficulty;
        target.AccuracyDifficulty = AccuracyDifficulty;
        target.HealthDifficulty = HealthDifficulty;
        target.EnableVisualization = AudioVisualizations;

        // TODO: this isn't being recalculated anywhere
        target.Hash = ChartHash;

        target.Filters ??= new RealmMapFilters();
        target.Filters.UpdateFilters(this);

        target.MapSet.Cover = CoverFile;
    }
}
