using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using JetBrains.Annotations;
using Newtonsoft.Json;
using osu.Framework.Logging;

namespace fluXis.Game.Utils;

public class MapUtils
{
    [CanBeNull]
    public static MapInfo LoadFromPath(string path)
    {
        try
        {
            MapInfo map = JsonConvert.DeserializeObject<MapInfo>(File.ReadAllText(path));
            return map;
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to load map from path: " + path);
            return null;
        }
    }

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

    public static string GetHash(string input) => BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(input))).Replace("-", "").ToLower();
}
