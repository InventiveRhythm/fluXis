using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Hashing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using fluXis.Database.Maps;
using fluXis.Localization.Categories;
using fluXis.Map;
using fluXis.Map.Structures;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events;
using fluXis.Map.Structures.Events.Playfields;
using fluXis.Map.Structures.Events.Scrolling;
using fluXis.Online.API.Models.Maps;
using fluXis.Utils.Attributes;
using osu.Framework.Extensions;
using osu.Framework.Localisation;

namespace fluXis.Utils;

public static class MapUtils
{
    public static int CompareMap(RealmMap first, RealmMap second, SortingMode mode, bool inverse = false)
    {
        if (first is null) return 1;
        if (second is null) return -1;

        var result = mode switch
        {
            SortingMode.Title => compareTitle(first, second),
            SortingMode.Artist => compareArtist(first, second),
            SortingMode.Length => compareLength(first, second),
            SortingMode.DateAdded => second.MapSet.DateAdded.CompareTo(first.MapSet.DateAdded),
            SortingMode.Difficulty => compareDifficulty(first, second),
            _ => 0
        };

        if (result == 0)
            result = first.GetHashCode().CompareTo(second.GetHashCode());

        if (inverse)
            result = -result;

        return result;
    }

    public static int CompareSets(RealmMapSet first, RealmMapSet second, SortingMode mode, bool inverse = false)
    {
        if (first is null) return 1;
        if (second is null) return -1;

        var result = mode switch
        {
            SortingMode.Title => compareTitle(first.LowestDifficulty, second.LowestDifficulty),
            SortingMode.Artist => compareArtist(first.LowestDifficulty, second.LowestDifficulty),
            SortingMode.Length => compareLength(first, second),
            SortingMode.DateAdded => second.DateAdded.CompareTo(first.DateAdded),
            SortingMode.Difficulty => compareDifficulty(first, second),
            _ => 0
        };

        if (result == 0)
            result = first.GetHashCode().CompareTo(second.GetHashCode());

        if (inverse)
            result = -result;

        return result;
    }

    private static int compareTitle(RealmMap first, RealmMap second)
    {
        if (first?.Metadata is null) return 1;
        if (second?.Metadata is null) return -1;

        var result = string.Compare(first.Metadata.SortingTitle, second.Metadata.SortingTitle, StringComparison.OrdinalIgnoreCase);

        if (result != 0)
            return result;

        if (first.MapSet.ID == second.MapSet.ID)
            return compareDifficulty(first, second);

        return CompareSets(first.MapSet, second.MapSet, SortingMode.DateAdded);
    }

    private static int compareArtist(RealmMap first, RealmMap second)
    {
        if (first.Metadata is null) return 1;
        if (second.Metadata is null) return -1;

        var result = string.Compare(first.Metadata.SortingArtist, second.Metadata.SortingArtist, StringComparison.OrdinalIgnoreCase);

        if (result != 0)
            return result;

        if (first.MapSet.ID == second.MapSet.ID)
            return compareDifficulty(first, second);

        return CompareMap(first, second, SortingMode.Title);
    }

    private static int compareLength(RealmMapSet first, RealmMapSet second)
    {
        var firstHighest = first.Maps.MaxBy(x => x.Filters?.Length);
        var secondHighest = second.Maps.MaxBy(x => x.Filters?.Length);
        return compareLength(firstHighest, secondHighest);
    }

    private static int compareLength(RealmMap first, RealmMap second)
    {
        if (first.Filters is null) return 1;
        if (second.Filters is null) return -1;

        var result = first.Filters.Length.CompareTo(second.Filters.Length);
        return result != 0 ? result : compareTitle(first, second);
    }

    private static int compareDifficulty(RealmMap first, RealmMap second)
    {
        if (first.Filters is null) return 1;
        if (second.Filters is null) return -1;

        var firstHighest = first.Rating;
        var secondHighest = second.Rating;

        var result = firstHighest.CompareTo(secondHighest);

        if (result != 0)
            return result;

        firstHighest = first.Filters?.NotesPerSecond ?? 0;
        secondHighest = second.Filters?.NotesPerSecond ?? 0;
        result = firstHighest.CompareTo(secondHighest);

        if (result != 0)
            return result;

        return string.Compare(first.Difficulty, second.Difficulty, StringComparison.OrdinalIgnoreCase);
    }

    private static int compareDifficulty(RealmMapSet first, RealmMapSet second)
    {
        var firstLowest = first.Maps.MaxBy(x => x.Rating);
        var secondLowest = second.Maps.MaxBy(x => x.Rating);
        return compareDifficulty(firstLowest, secondLowest);
    }

    public static RealmMapFilters UpdateFilters(this RealmMapFilters filters, PlayableMap map)
    {
        filters.Reset();

        foreach (var hitObject in map.ObjectsOfType<HitObject>())
        {
            filters.Length = (float)Math.Max(filters.Length, hitObject.GetEndTime());

            // TODO: fix :bellfast:
            /*if (hitObject.LongNote)
                filters.LongNoteCount++;
            else if (hitObject.Landmine)
                filters.LandmineCount++;
            else
                filters.NoteCount++;*/
        }

        filters.NotesPerSecond = GetNps(map.ObjectsOfType<HitObject>());

        foreach (var timingPoint in map.ObjectsOfType<TimingPoint>())
        {
            if (filters.BPMMin == 0)
                filters.BPMMin = timingPoint.BPM;

            filters.BPMMin = Math.Min(filters.BPMMin, timingPoint.BPM);
            filters.BPMMax = Math.Max(filters.BPMMax, timingPoint.BPM);
        }

        if (map.ObjectsOfType<ScrollVelocity>().Length >= 20)
            filters.Effects |= MapEffectType.ScrollVelocity;

        filters.Effects |= GetEffects(map);
        return filters;
    }

    public static RealmMapFilters GetMapFilters(PlayableMap map)
        => new RealmMapFilters().UpdateFilters(map);

    public static MapEffectType GetEffects(PlayableMap map)
    {
        MapEffectType effects = 0;

        if (map.ObjectsOfType<ScrollVelocity>().Length > 20)
            effects |= MapEffectType.ScrollVelocity;

        if (map.ObjectsOfType<LaneSwitchEvent>().Length > 0)
            effects |= MapEffectType.LaneSwitch;

        if (map.ObjectsOfType<FlashEvent>().Length > 0)
            effects |= MapEffectType.Flash;

        if (map.ObjectsOfType<ColorFadeEvent>().Length > 0)
            effects |= MapEffectType.ColorFade;

        if (map.ObjectsOfType<PulseEvent>().Length > 0)
            effects |= MapEffectType.Pulse;

        if (map.ObjectsOfType<PlayfieldMoveEvent>().Length > 0)
            effects |= MapEffectType.PlayfieldMove;

        if (map.ObjectsOfType<PlayfieldScaleEvent>().Length > 0)
            effects |= MapEffectType.PlayfieldScale;

        if (map.ObjectsOfType<PlayfieldRotateEvent>().Length > 0)
            effects |= MapEffectType.PlayfieldRotate;

        /*if (events.PlayfieldFadeEvents.Count > 0)
            effects |= MapEffectType.PlayfieldFade;*/

        if (map.ObjectsOfType<ShakeEvent>().Length > 0)
            effects |= MapEffectType.Shake;

        if (map.ObjectsOfType<ShaderEvent>().Length > 0)
            effects |= MapEffectType.Shader;

        if (map.ObjectsOfType<BeatPulseEvent>().Length > 0)
            effects |= MapEffectType.BeatPulse;

        if (map.ObjectsOfType<LayerFadeEvent>().Length > 0)
            effects |= MapEffectType.LayerFade;

        if (map.ObjectsOfType<HitObjectEaseEvent>().Length > 0)
            effects |= MapEffectType.HitObjectEase;

        if (map.ObjectsOfType<ScrollMultiplierEvent>().Length > 0)
            effects |= MapEffectType.ScrollMultiply;

        if (map.ObjectsOfType<TimeOffsetEvent>().Length > 0)
            effects |= MapEffectType.TimeOffset;

        return effects;
    }

    public static float GetNps(HitObject[] hits)
    {
        if (hits.Length == 0) return 0;

        Dictionary<int, float> seconds = new Dictionary<int, float>();

        foreach (var hitObject in hits)
        {
            int second = (int)hitObject.Time / 1000;
            var value = hitObject.DensityContribution;

            if (!seconds.TryAdd(second, value))
                seconds[second] += value;
        }

        return seconds.Average(x => x.Value);
    }

    public static string GetHash(string input) => BitConverter.ToString(SHA256.HashData(Encoding.UTF8.GetBytes(input))).Replace("-", "").ToLower();
    public static string GetHash(Stream input) => BitConverter.ToString(SHA256.Create().ComputeHash(input)).Replace("-", "").ToLower();
    public static string GetHash(byte[] input) => BitConverter.ToString(SHA256.HashData(input)).Replace("-", "").ToLower();

    public static string GetXXHash(string input) => BitConverter.ToString(XxHash3.Hash(Encoding.UTF8.GetBytes(input))).Replace("-", "").ToLower();
    public static string GetXXHash(Stream input) => BitConverter.ToString(XxHash3.Hash(input.ReadAllBytesToArray())).Replace("-", "").ToLower();
    public static string GetXXHash(byte[] input) => BitConverter.ToString(XxHash3.Hash(input)).Replace("-", "").ToLower();

    public static float GetDifficulty(float difficulty, float min, float mid, float max)
    {
        if (difficulty > 5)
            return mid + (max - mid) * getDifficulty(difficulty);
        if (difficulty < 5)
            return mid + (mid - min) * getDifficulty(difficulty);

        return mid;
    }

    private static float getDifficulty(float difficulty) => (difficulty - 5) / 5;

    public enum SortingMode
    {
        [Icon(0xE340)]
        [LocalisableDescription(typeof(SongSelectStrings), nameof(SongSelectStrings.SortByTitle))]
        Title,

        [Icon(0xE75C)]
        [LocalisableDescription(typeof(SongSelectStrings), nameof(SongSelectStrings.SortByArtist))]
        Artist,

        [Icon(0xE19A)]
        [LocalisableDescription(typeof(SongSelectStrings), nameof(SongSelectStrings.SortByLength))]
        Length,

        [Icon(0xE108)]
        [LocalisableDescription(typeof(SongSelectStrings), nameof(SongSelectStrings.SortByDateAdded))]
        DateAdded,

        [Icon(0xE2F6)]
        [LocalisableDescription(typeof(SongSelectStrings), nameof(SongSelectStrings.SortByDifficulty))]
        Difficulty
    }

    public enum GroupingMode
    {
        [Icon(0xE1DA)]
        [LocalisableDescription(typeof(SongSelectStrings), nameof(SongSelectStrings.GroupByDefault))]
        Default,

        [Icon(0xE32A)]
        [LocalisableDescription(typeof(SongSelectStrings), nameof(SongSelectStrings.GroupByNothing))]
        Nothing
    }
}
