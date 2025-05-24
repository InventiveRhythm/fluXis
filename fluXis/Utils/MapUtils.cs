using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using fluXis.Database.Maps;
using fluXis.Localization.Categories;
using fluXis.Map;
using fluXis.Map.Structures;
using fluXis.Online.API.Models.Maps;
using fluXis.Utils.Attributes;
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

    public static RealmMapFilters UpdateFilters(this RealmMapFilters filters, MapInfo map, MapEvents events)
    {
        filters.Reset();

        foreach (var hitObject in map.HitObjects)
        {
            filters.Length = (float)Math.Max(filters.Length, hitObject.EndTime);

            if (hitObject.LongNote)
                filters.LongNoteCount++;
            else
                filters.NoteCount++;
        }

        filters.NotesPerSecond = GetNps(map.HitObjects);

        foreach (var timingPoint in map.TimingPoints)
        {
            if (filters.BPMMin == 0)
                filters.BPMMin = timingPoint.BPM;

            filters.BPMMin = Math.Min(filters.BPMMin, timingPoint.BPM);
            filters.BPMMax = Math.Max(filters.BPMMax, timingPoint.BPM);
        }

        if (map.ScrollVelocities.Count >= 20)
            filters.Effects |= MapEffectType.ScrollVelocity;

        if (events != null)
            filters.Effects = GetEffects(events);

        return filters;
    }

    public static RealmMapFilters GetMapFilters(MapInfo map, MapEvents events)
        => new RealmMapFilters().UpdateFilters(map, events);

    public static MapEffectType GetEffects(MapEvents events)
    {
        MapEffectType effects = 0;

        if (events.LaneSwitchEvents.Count > 0)
            effects |= MapEffectType.LaneSwitch;

        if (events.FlashEvents.Count > 0)
            effects |= MapEffectType.Flash;

        if (events.PulseEvents.Count > 0)
            effects |= MapEffectType.Pulse;

        if (events.PlayfieldMoveEvents.Count > 0)
            effects |= MapEffectType.PlayfieldMove;

        if (events.PlayfieldScaleEvents.Count > 0)
            effects |= MapEffectType.PlayfieldScale;

        if (events.PlayfieldRotateEvents.Count > 0)
            effects |= MapEffectType.PlayfieldRotate;

        /*if (events.PlayfieldFadeEvents.Count > 0)
            effects |= MapEffectType.PlayfieldFade;*/

        if (events.ShakeEvents.Count > 0)
            effects |= MapEffectType.Shake;

        if (events.ShaderEvents.Count > 0)
            effects |= MapEffectType.Shader;

        if (events.BeatPulseEvents.Count > 0)
            effects |= MapEffectType.BeatPulse;

        if (events.LayerFadeEvents.Count > 0)
            effects |= MapEffectType.LayerFade;

        if (events.HitObjectEaseEvents.Count > 0)
            effects |= MapEffectType.HitObjectEase;

        if (events.ScrollMultiplyEvents.Count > 0)
            effects |= MapEffectType.ScrollMultiply;

        if (events.TimeOffsetEvents.Count > 0)
            effects |= MapEffectType.TimeOffset;

        return effects;
    }

    public static float GetNps(List<HitObject> hitObjects)
    {
        if (hitObjects.Count == 0) return 0;

        Dictionary<int, float> seconds = new Dictionary<int, float>();

        foreach (var hitObject in hitObjects)
        {
            int second = (int)hitObject.Time / 1000;

            var value = hitObject.Type switch
            {
                1 => 0.1f, // tick
                _ => 1
            };

            if (!seconds.TryAdd(second, value))
                seconds[second] += value;
        }

        return seconds.Average(x => x.Value);
    }

    public static string GetHash(string input) => BitConverter.ToString(SHA256.HashData(Encoding.UTF8.GetBytes(input))).Replace("-", "").ToLower();
    public static string GetHash(Stream input) => BitConverter.ToString(SHA256.Create().ComputeHash(input)).Replace("-", "").ToLower();
    public static string GetHash(byte[] input) => BitConverter.ToString(SHA256.HashData(input)).Replace("-", "").ToLower();

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
        [Icon(0xf031)]
        [LocalisableDescription(typeof(SongSelectStrings), nameof(SongSelectStrings.SortByTitle))]
        Title,

        [Icon(0xf031)]
        [LocalisableDescription(typeof(SongSelectStrings), nameof(SongSelectStrings.SortByArtist))]
        Artist,

        [Icon(0xf017)]
        [LocalisableDescription(typeof(SongSelectStrings), nameof(SongSelectStrings.SortByLength))]
        Length,

        [Icon(0xf073)]
        [LocalisableDescription(typeof(SongSelectStrings), nameof(SongSelectStrings.SortByDateAdded))]
        DateAdded,

        [Icon(0xf1b2)]
        [LocalisableDescription(typeof(SongSelectStrings), nameof(SongSelectStrings.SortByDifficulty))]
        Difficulty
    }

    public enum GroupingMode
    {
        [Icon(0xf068)]
        [LocalisableDescription(typeof(SongSelectStrings), nameof(SongSelectStrings.GroupByDefault))]
        Default,

        [Icon(0xf068)]
        [LocalisableDescription(typeof(SongSelectStrings), nameof(SongSelectStrings.GroupByNothing))]
        Nothing
    }
}
