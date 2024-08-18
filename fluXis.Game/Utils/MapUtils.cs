using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using fluXis.Game.Map.Structures;

namespace fluXis.Game.Utils;

public static class MapUtils
{
    public static int CompareMap(RealmMap first, RealmMap second, SortingMode mode, bool inverse = false)
    {
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
        var result = string.Compare(first.Metadata.Title, second.Metadata.Title, StringComparison.OrdinalIgnoreCase);

        if (result != 0)
            return result;

        if (first.MapSet.ID == second.MapSet.ID)
            return compareDifficulty(first, second);

        return CompareSets(first.MapSet, second.MapSet, SortingMode.DateAdded);
    }

    private static int compareArtist(RealmMap first, RealmMap second)
    {
        var result = string.Compare(first.Metadata.Artist, second.Metadata.Artist, StringComparison.OrdinalIgnoreCase);

        if (result != 0)
            return result;

        if (first.MapSet.ID == second.MapSet.ID)
            return compareDifficulty(first, second);

        return CompareMap(first, second, SortingMode.Title);
    }

    private static int compareLength(RealmMapSet first, RealmMapSet second)
    {
        var firstHighest = first.Maps.MaxBy(x => x.Filters.Length);
        var secondHighest = second.Maps.MaxBy(x => x.Filters.Length);
        return compareLength(firstHighest, secondHighest);
    }

    private static int compareLength(RealmMap first, RealmMap second)
    {
        var result = first.Filters.Length.CompareTo(second.Filters.Length);
        return result != 0 ? result : CompareMap(first, second, SortingMode.Title);
    }

    private static int compareDifficulty(RealmMap first, RealmMap second)
    {
        var firstHighest = first.Filters.NotesPerSecond;
        var secondHighest = second.Filters.NotesPerSecond;
        var result = firstHighest.CompareTo(secondHighest);
        return result;
    }

    private static int compareDifficulty(RealmMapSet first, RealmMapSet second)
    {
        var firstLowest = first.Maps.MaxBy(x => x.Filters.NotesPerSecond);
        var secondLowest = second.Maps.MaxBy(x => x.Filters.NotesPerSecond);
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

        filters.NotesPerSecond = getNps(map.HitObjects);

        foreach (var timingPoint in map.TimingPoints)
        {
            if (filters.BPMMin == 0)
                filters.BPMMin = timingPoint.BPM;

            filters.BPMMin = Math.Min(filters.BPMMin, timingPoint.BPM);
            filters.BPMMax = Math.Max(filters.BPMMax, timingPoint.BPM);
        }

        foreach (var scrollVelocity in map.ScrollVelocities)
        {
            if (scrollVelocity.Multiplier != 1)
            {
                filters.Effects |= EffectType.ScrollVelocity;
                break;
            }
        }

        if (events != null)
            filters.Effects = getEffects(events);

        return filters;
    }

    private static EffectType getEffects(MapEvents events)
    {
        EffectType effects = 0;

        if (events.LaneSwitchEvents.Count > 0)
            effects |= EffectType.LaneSwitch;

        if (events.FlashEvents.Count > 0)
            effects |= EffectType.Flash;

        if (events.PulseEvents.Count > 0)
            effects |= EffectType.Pulse;

        if (events.PlayfieldMoveEvents.Count > 0)
            effects |= EffectType.PlayfieldMove;

        if (events.PlayfieldScaleEvents.Count > 0)
            effects |= EffectType.PlayfieldScale;

        if (events.PlayfieldRotateEvents.Count > 0)
            effects |= EffectType.PlayfieldRotate;

        if (events.PlayfieldFadeEvents.Count > 0)
            effects |= EffectType.PlayfieldFade;

        if (events.ShakeEvents.Count > 0)
            effects |= EffectType.Shake;

        if (events.ShaderEvents.Count > 0)
            effects |= EffectType.Shader;

        if (events.BeatPulseEvents.Count > 0)
            effects |= EffectType.BeatPulse;

        return effects;
    }

    public static RealmMapFilters GetMapFilters(MapInfo map, MapEvents events)
        => new RealmMapFilters().UpdateFilters(map, events);

    private static float getNps(List<HitObject> hitObjects)
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

    public static string GetHash(string input) => BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(input))).Replace("-", "").ToLower();
    public static string GetHash(Stream input) => BitConverter.ToString(SHA256.Create().ComputeHash(input)).Replace("-", "").ToLower();

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
        [Description("Title")]
        Title,

        [Description("Artist")]
        Artist,

        [Description("Length")]
        Length,

        [Description("Date Added")]
        DateAdded,

        [Description("Difficulty")]
        Difficulty
    }

    public enum GroupingMode
    {
        [Description("None")]
        None,

        [Description("Difficulty")]
        Difficulty
    }
}
