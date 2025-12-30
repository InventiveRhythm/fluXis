using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using fluXis.Map;
using fluXis.Map.Structures.Bases;
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

    private MapInfo map { get; }
    private MapEvents events { get; }
    private Lua lua { get; }

    public LuaMap(MapInfo map, Lua lua = null)
    {
        this.map = map;
        this.lua = lua;
        events = map.GetMapEvents();
    }

    [LuaMember(Name = "NotesInRange")]
    public LuaTable GetNotesInRange(double startTime, double endTime)
        => getInRange(map.HitObjects, startTime, endTime).ToLuaTable(lua);

    [LuaMember(Name = "TimingPointsInRange")]
    public LuaTable GetTimingPointsInRange(double startTime, double endTime)
        => getInRange(map.TimingPoints, startTime, endTime).ToLuaTable(lua);

    [LuaMember(Name = "ScrollVelocitiesInRange")]
    public LuaTable GetScrollVelocitiesInRange(double startTime, double endTime)
        => getInRange(map.ScrollVelocities, startTime, endTime).ToLuaTable(lua);

    [LuaMember(Name = "HitSoundFadesInRange")]
    public LuaTable GetHitSoundFadesInRange(double startTime, double endTime)
        => getInRange(map.HitSoundFades, startTime, endTime).ToLuaTable(lua);

    [LuaMember(Name = "EventsInRange")]
    public LuaTable GetEventsInRange(double startTime, double endTime, [LuaCustomType(typeof(IMapEvent))] string eventType)
    {
        var type = lookup.GetValueOrDefault(eventType);
        if (type is null) return new List<object>().ToLuaTable(lua);

        var prop = events.AllListProperties.FirstOrDefault(x => x.PropertyType.GenericTypeArguments.FirstOrDefault() == type);
        if (prop is null) throw new InvalidOperationException("MapEvents does not have requested Type.");

        var list = prop.GetValue(events) ?? throw new NullReferenceException();
        var method = GetType().GetMethod(nameof(getInRange), BindingFlags.Static | BindingFlags.NonPublic);
        var gen = method!.MakeGenericMethod(type);
        var result = gen.Invoke(null, new[] { list, startTime, endTime }) as IEnumerable;
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
