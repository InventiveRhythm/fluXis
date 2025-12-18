using System.Collections.Generic;
using System.Linq;
using fluXis.Map;
using fluXis.Map.Structures.Bases;
using fluXis.Scripting.Attributes;
using fluXis.Utils.Extensions;
using NLua;

namespace fluXis.Scripting.Models.Storyboarding;

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

    [LuaMember(Name = "EventsInRange")]
    public LuaTable GetEventsInRange(double startTime, double endTime, string eventType)
    {
        List<ITimedObject> effects = eventType switch
        {
            "LaneSwitch" => getInRange(events.LaneSwitchEvents, startTime, endTime).Cast<ITimedObject>().ToList(),
            "Flash" => getInRange(events.FlashEvents, startTime, endTime).Cast<ITimedObject>().ToList(),
            "Pulse" => getInRange(events.PulseEvents, startTime, endTime).Cast<ITimedObject>().ToList(),
            "PlayfieldMove" => getInRange(events.PlayfieldMoveEvents, startTime, endTime).Cast<ITimedObject>().ToList(),
            "PlayfieldScale" => getInRange(events.PlayfieldScaleEvents, startTime, endTime).Cast<ITimedObject>().ToList(),
            "PlayfieldRotate" => getInRange(events.PlayfieldRotateEvents, startTime, endTime).Cast<ITimedObject>().ToList(),
            "Shake" => getInRange(events.ShakeEvents, startTime, endTime).Cast<ITimedObject>().ToList(),
            "Shader" => getInRange(events.ShaderEvents, startTime, endTime).Cast<ITimedObject>().ToList(),
            "BeatPulse" => getInRange(events.BeatPulseEvents, startTime, endTime).Cast<ITimedObject>().ToList(),
            "LayerFade" => getInRange(events.LayerFadeEvents, startTime, endTime).Cast<ITimedObject>().ToList(),
            "HitObjectEase" => getInRange(events.HitObjectEaseEvents, startTime, endTime).Cast<ITimedObject>().ToList(),
            "ScrollMultiplier" => getInRange(events.ScrollMultiplyEvents, startTime, endTime).Cast<ITimedObject>().ToList(),
            "TimeOffset" => getInRange(events.TimeOffsetEvents, startTime, endTime).Cast<ITimedObject>().ToList(),
            "ColorFade" => getInRange(events.ColorFadeEvents, startTime, endTime).Cast<ITimedObject>().ToList(),
            "CameraMove" => getInRange(events.CameraMoveEvents, startTime, endTime).Cast<ITimedObject>().ToList(),
            "CameraScale" => getInRange(events.CameraScaleEvents, startTime, endTime).Cast<ITimedObject>().ToList(),
            "CameraRotate" => getInRange(events.CameraRotateEvents, startTime, endTime).Cast<ITimedObject>().ToList(),
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
