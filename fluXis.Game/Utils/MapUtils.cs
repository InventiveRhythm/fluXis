using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using Newtonsoft.Json;

namespace fluXis.Game.Utils;

public static class MapUtils
{
    public static RealmMapFilters GetMapFilters(MapInfo map, MapEvents events)
    {
        RealmMapFilters filters = new RealmMapFilters();

        foreach (var hitObject in map.HitObjects)
        {
            filters.Length = Math.Max(filters.Length, hitObject.HoldEndTime);

            if (hitObject.IsLongNote())
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
                filters.HasScrollVelocity = true;
                break;
            }
        }

        filters.HasLaneSwitch = events.LaneSwitchEvents.Count > 0;
        filters.HasFlash = events.FlashEvents.Count > 0;

        return filters;
    }

    private static float getNps(List<HitObjectInfo> hitObjects)
    {
        if (hitObjects.Count == 0) return 0;

        Dictionary<int, int> seconds = new Dictionary<int, int>();

        foreach (var hitObject in hitObjects)
        {
            int second = (int)hitObject.Time / 1000;
            if (seconds.ContainsKey(second))
                seconds[second]++;
            else
                seconds.Add(second, 1);
        }

        return (float)seconds.Average(x => x.Value);
    }

    public static string Serialize(this MapInfo map, bool indent = false)
    {
        var json = JsonConvert.SerializeObject(map, indent ? Formatting.Indented : Formatting.None, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        });

        return json;
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
}
