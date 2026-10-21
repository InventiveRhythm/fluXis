using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using fluXis.Map;
using fluXis.Map.Structures;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events;
using fluXis.Scripting.Attributes;
using fluXis.Utils.Extensions;
using NLua;

namespace fluXis.Scripting.Models.Storyboarding;

[LuaDefinition("map", Name = "map", Public = true)]
public class LuaMap : ILuaModel
{
    private static Dictionary<string, Type> lookup { get; }

    static LuaMap()
    {
        var types = GetMapEventTypes();
        lookup = types.ToDictionary(x => x.Name.Replace("Event", ""), x => x);
    }

    private PlayableMap map { get; }
    private Lua lua { get; }

    public LuaMap(PlayableMap map, Lua lua = null)
    {
        this.map = map;
        this.lua = lua;
    }

#nullable enable

    /// <summary>
    /// Get all or specific types of Objects in a range. Leave 'types' parameter if you want all Objects.
    /// Example:
    /// NotesInRange(0, 1000, { "flux:keys/normal", "flux:keys/long" })
    /// </summary>
    [LuaMember(Name = "ObjectsInRange")]
    public LuaTable GetObjectsInRange(double startTime, double endTime, LuaTable? types = null)
    {
        var objects = getInRange([.. map.Objects], startTime, endTime);

        if (types is null)
            return objects.ToLuaTable(lua);

        var typesArr = types.Values.Cast<string>().ToArray();

        return objects.Where(h =>
        {
            string location = (h.GetType().GetCustomAttribute<rhym.ResourceLocationAttribute>()
                               ?? throw new InvalidOperationException("Object should have a type.")).Location;

            return typesArr.Contains(location);
        }).ToLuaTable(lua);
    }

    /// <summary>
    /// Get all or specific types of Hitobjects in a range. Leave 'type' parameter if you want all HitObjects.
    /// 'Normal' HitObjectType will also give long notes not just normal notes.
    /// </summary>
    [LuaMember(Name = "NotesInRange")]
    [LuaVersion(Until = 2)]
    [Obsolete("Use map:ObjectsInRange(...) instead")]
    public LuaTable GetNotesInRangeLegacy(double startTime, double endTime, string? type = null)
    {
        var objects = getInRange([.. map.ObjectsOfType<HitObject>()], startTime, endTime);

        if (type is null)
            return objects.ToLuaTable(lua);

        return objects.Where(h =>
        {
            string location = (h.GetType().GetCustomAttribute<rhym.ResourceLocationAttribute>()
                               ?? throw new InvalidOperationException("Object should have a type.")).Location;

            string targetLocation = LuaHitObject.LegacyToKeysGameMode(type);

            return location == targetLocation;
        }).ToLuaTable(lua);
    }

#nullable restore

    [LuaMember(Name = "TimingPointsInRange")]
    [LuaVersion(Until = 2)]
    [Obsolete("Use map:ObjectsInRange(...) instead")]
    public LuaTable GetTimingPointsInRange(double startTime, double endTime)
        => getInRange([.. map.ObjectsOfType<TimingPoint>()], startTime, endTime).ToLuaTable(lua);

    [LuaMember(Name = "ScrollVelocitiesInRange")]
    [LuaVersion(Until = 2)]
    [Obsolete("Use map:ObjectsInRange(...) instead")]
    public LuaTable GetScrollVelocitiesInRange(double startTime, double endTime)
        => getInRange([.. map.ObjectsOfType<ScrollVelocity>()], startTime, endTime).ToLuaTable(lua);

    [LuaMember(Name = "HitSoundFadesInRange")]
    [LuaVersion(Until = 2)]
    [Obsolete("Use map:ObjectsInRange(...) instead")]
    public LuaTable GetHitSoundFadesInRange(double startTime, double endTime)
        => getInRange([.. map.ObjectsOfType<HitSoundFade>()], startTime, endTime).ToLuaTable(lua);

    [LuaMember(Name = "EventsInRange")]
    [LuaVersion(Until = 2)]
    [Obsolete("Use map:ObjectsInRange(...) instead")]
    public LuaTable GetEventsInRange(double startTime, double endTime, [LuaCustomType(typeof(IMapEvent))] string eventType)
    {
        var type = lookup.GetValueOrDefault(eventType);
        if (type is null) return new List<object>().ToLuaTable(lua);

        var list = map.ObjectsOfType(type).ToList();
        var method = GetType().GetMethod(nameof(getInRange), BindingFlags.Static | BindingFlags.NonPublic);
        var gen = method!.MakeGenericMethod(type);
        var result = gen.Invoke(null, [list, startTime, endTime]) as IEnumerable;
        var table = result.ToLuaTable(lua);
        return table;
    }

    private static List<T> getInRange<T>(List<T> list, double startTime, double endTime) where T : ITimedObject
    {
        int startIdx = list.FindIndex(x => x.Time >= startTime);
        int endIdx = list.FindLastIndex(x => x.Time <= endTime);

        if (startIdx == -1 || endIdx == -1 || startIdx > endIdx)
            return new List<T>();

        return list.GetRange(startIdx, endIdx - startIdx + 1);
    }

    public static List<Type> GetMapEventTypes()
    {
        var asm = typeof(IMapEvent).Assembly;
        var types = asm.GetTypes();
        var wInterface = types.Where(x => x.GetInterfaces().Contains(typeof(IMapEvent)));
        return wInterface.Where(x => x.IsClass && !x.IsAbstract).ToList();
    }
}
