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
    public static void Sort(this List<RealmMapSet> list, SortingMode mode, bool inverse = false)
        => list.Sort((a, b) => CompareSets(a, b, mode, inverse));

    public static int CompareSets(RealmMapSet first, RealmMapSet second, SortingMode mode, bool inverse = false)
    {
        var result = mode switch
        {
            SortingMode.Title => compareTitle(first, second),
            SortingMode.Artist => compareArtist(first, second),
            SortingMode.Length => compareLength(first, second),
            SortingMode.DateAdded => second.DateAdded.CompareTo(first.DateAdded),
            _ => 0
        };

        if (inverse)
            result = -result;

        return result;
    }

    private static int compareTitle(RealmMapSet first, RealmMapSet second)
    {
        var result = string.Compare(first.Metadata.Title, second.Metadata.Title, StringComparison.OrdinalIgnoreCase);

        // fall back to date added if the title is the same
        return result != 0 ? result : CompareSets(first, second, SortingMode.DateAdded);
    }

    private static int compareArtist(RealmMapSet first, RealmMapSet second)
    {
        var result = string.Compare(first.Metadata.Artist, second.Metadata.Artist, StringComparison.OrdinalIgnoreCase);
        return result != 0 ? result : CompareSets(first, second, SortingMode.Title);
    }

    private static int compareLength(RealmMapSet first, RealmMapSet second)
    {
        var firstHighest = first.Maps.MaxBy(x => x.Filters.Length).Filters.Length;
        var secondHighest = second.Maps.MaxBy(x => x.Filters.Length).Filters.Length;
        var result = firstHighest.CompareTo(secondHighest);
        return result != 0 ? result : CompareSets(first, second, SortingMode.Title);
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
        DateAdded
    }
}
