using System.Collections.Generic;
using System.Linq;
using fluXis.Map;
using fluXis.Map.Structures.Bases;
using fluXis.Online.API.Models.Maps;
using fluXis.Scripting.Attributes;
using fluXis.Utils.Extensions;
using NLua;

namespace fluXis.Scripting.Models.Skinning;

[LuaDefinition("map", Name = "map", Public = true)]
public class LuaMap : ILuaModel
{
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

    [LuaMember(Name = "EffectsInRange")]
    public LuaTable GetEffectsInRange(double startTime, double endTime, ulong effectTypeValue)
    {
        MapEffectType effectType = (MapEffectType)effectTypeValue;

        List<ITimedObject> effects = effectType switch
        {
            MapEffectType.ScrollVelocity => getInRange(map.ScrollVelocities, startTime, endTime).Cast<ITimedObject>().ToList(),
            MapEffectType.LaneSwitch => getInRange(events.LaneSwitchEvents, startTime, endTime).Cast<ITimedObject>().ToList(),
            MapEffectType.Flash => getInRange(events.FlashEvents, startTime, endTime).Cast<ITimedObject>().ToList(),
            MapEffectType.Pulse => getInRange(events.PulseEvents, startTime, endTime).Cast<ITimedObject>().ToList(),
            MapEffectType.PlayfieldMove => getInRange(events.PlayfieldMoveEvents, startTime, endTime).Cast<ITimedObject>().ToList(),
            MapEffectType.PlayfieldScale => getInRange(events.PlayfieldScaleEvents, startTime, endTime).Cast<ITimedObject>().ToList(),
            MapEffectType.PlayfieldRotate => getInRange(events.PlayfieldRotateEvents, startTime, endTime).Cast<ITimedObject>().ToList(),
            MapEffectType.Shake => getInRange(events.ShakeEvents, startTime, endTime).Cast<ITimedObject>().ToList(),
            MapEffectType.Shader => getInRange(events.ShaderEvents, startTime, endTime).Cast<ITimedObject>().ToList(),
            MapEffectType.BeatPulse => getInRange(events.BeatPulseEvents, startTime, endTime).Cast<ITimedObject>().ToList(),
            MapEffectType.LayerFade => getInRange(events.LayerFadeEvents, startTime, endTime).Cast<ITimedObject>().ToList(),
            MapEffectType.HitObjectEase => getInRange(events.HitObjectEaseEvents, startTime, endTime).Cast<ITimedObject>().ToList(),
            MapEffectType.ScrollMultiply => getInRange(events.ScrollMultiplyEvents, startTime, endTime).Cast<ITimedObject>().ToList(),
            MapEffectType.TimeOffset => getInRange(events.TimeOffsetEvents, startTime, endTime).Cast<ITimedObject>().ToList(),
            MapEffectType.ColorFade => getInRange(events.ColorFadeEvents, startTime, endTime).Cast<ITimedObject>().ToList(),
            _ => new List<ITimedObject>()
        };
        
        return effects.ToLuaTable(lua);
    }
    
    
    private static List<T> getInRange<T>(List<T> list, double startTime, double endTime) where T : ITimedObject
    {
        int startIdx = list.FindIndex(x => x.Time >= startTime);
        int endIdx = list.FindLastIndex(x => x.Time <= endTime);

        if (startIdx == -1 || endIdx == -1 || startIdx > endIdx) 
            return new List<T>();

        return list.GetRange(startIdx, endIdx - startIdx + 1);
    }
}
