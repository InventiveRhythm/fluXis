using System;
using System.Reflection;
using fluXis.Map.Structures;
using fluXis.Map.Structures.Bases;
using fluXis.Scripting.Attributes;
using NLua;

namespace fluXis.Scripting.Models;

[LuaDefinition("struct", Name = "HitObject", Public = true)]
public class LuaHitObject : ILuaModel
{
    [LuaHide]
    private readonly HitObject obj;

    public LuaHitObject(HitObject obj)
    {
        this.obj = obj;
    }

    [LuaMember(Name = "time")]
    public double Time => obj.Time;

    [LuaMember(Name = "lane")]
    public int Lane => obj.Lane;

    [LuaMember(Name = "sample")]
    public string Sample => obj.Sample;

    [LuaMember(Name = "type")]
    [LuaVersion(Since = 3)]
    public string Type => (obj.GetType().GetCustomAttribute<rhym.ResourceLocationAttribute>()
                           ?? throw new InvalidOperationException("Object should have a type.")).Location;

    [LuaMember(Name = "duration")]
    public double Duration => (obj as IHasDuration)?.Duration ?? 0;

    #region Legay Stuff

    [LuaMember(Name = "type")]
    [LuaVersion(Until = 2)]
    public int TypeLegacy => (int)keysGameModeToLegacy(Type);

    [LuaMember(Name = "holdTime")]
    [LuaVersion(Until = 2)]
    [Obsolete("Use 'duration' instead")]
    public double HoldTime => Duration;

    [LuaMember(Name = "hitSound")]
    [LuaVersion(Until = 2)]
    [Obsolete("Use 'sample' instead")]
    public string HitSound => Sample;

    [LuaMember(Name = "visualLane")]
    [LuaVersion(Until = 2)]
    [Obsolete("No longer supported and always returns 1")]
    public int VisualLane => 1;

    [LuaHide]
    private static LuaHitObjectTypeLegacy keysGameModeToLegacy(string location)
    {
        // legacy script versions only supported the keys gamemode
        return location switch
        {
            "flux:keys/note" or "flux:keys/long" => LuaHitObjectTypeLegacy.Normal,
            "flux:keys/tick" => LuaHitObjectTypeLegacy.Tick,
            "flux:keys/mine" => LuaHitObjectTypeLegacy.Landmine,
            _ => LuaHitObjectTypeLegacy.Normal
        };
    }

    // Just using strings instead of the enum to save an extra conversion step
    [LuaHide]
    public static string LegacyToKeysGameMode(string type)
    {
        return type switch
        {
            "Tick" => "flux:keys/tick",
            "Landmine" => "flux:keys/mine",
            _ => "flux:keys/note"
        };
    }

    #endregion
}
