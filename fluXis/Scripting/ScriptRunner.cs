using System;
using System.Text;
using fluXis.Map;
using fluXis.Scripting.Attributes;
using fluXis.Scripting.Models;
using JetBrains.Annotations;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using osu.Framework.Utils;
using Lua = NLua.Lua;
using LuaFunction = NLua.LuaFunction;

namespace fluXis.Scripting;

[LuaDefinition("shared", Hide = true)]
public class ScriptRunner
{
    public static Logger Logger { get; } = Logger.GetLogger("scripting");

    [CanBeNull]
    protected MapInfo Map { get; set; }

    public Action<string, string, string, object> DefineParameter;
    protected Lua Lua { get; }
    protected int Version { get; private set; }

    protected ScriptRunner()
    {
        var state = new KeraLua.Lua(false);
        state.RequireF("", LuaNative.luaopen_base, true);
        state.RequireF("table", LuaNative.luaopen_table, true);
        state.RequireF("string", LuaNative.luaopen_string, true);
        state.RequireF("math", LuaNative.luaopen_math, true);

        state.NewTable();
        state.SetGlobal("luanet");

        Lua = new Lua(state);
        Lua.State.Encoding = Encoding.UTF8;
        Lua.DoString("luanet = nil"); // this just lets you load any type because of course it does. library written by a monkey with typewriter on drugs

        AddField("mathf", new LuaMath());
        AddField("FFTParameters", new LuaFFTParameters());

        AddFunction("print", print);
        AddFunction("RandomRange", randomRange);
        AddFunction("Easing", (string str) => Enum.TryParse(str, out Easing easing) ? easing : Easing.None);
        AddFunction("Vector2", (float x, float y) => new LuaVector2(x, y));
        AddFunction("Color4", (float r, float g, float b, float a) => new LuaColor4(r, g, b, a));
        AddFunction("Vector2Zero", () => new LuaVector2(0, 0));
        AddFunction("Vector2One", () => new LuaVector2(1, 1));
        AddFunction("BPMAtTime", findBpm);

        AddFunction("DefineParameter", defineParameter);
        AddFunction("SetVersion", setVersion);

        Lua.DoString("import = function() end"); // disable importing
    }

    public void AddFunction(string name, Delegate function)
        => Lua.RegisterFunction(name, function.Target, function.Method);

    [CanBeNull]
    protected LuaFunction GetFunction(string name)
    {
        try
        {
            return Lua.GetFunction(name);
        }
        catch
        {
            return null;
        }
    }

    protected void AddField(string name, ILuaModel value) => Lua[name] = value;

    public void Run(string code)
    {
        Lua.DoString(code);
    }

    [LuaGlobal(Name = "SetVersion")]
    private void setVersion(double v) => Version = (int)v;

    [LuaGlobal(Name = "RandomRange")]
    private int randomRange(int from, int to) => RNG.Next(from, to + 1);

    [LuaGlobal(Name = "BPMAtTime")]
    private float findBpm(double time)
    {
        var point = Map?.GetTimingPoint(time) ?? throw new InvalidOperationException("Tried to call findBpm without a map!");
        return point.BPM;
    }

#nullable enable
    [LuaGlobal(Name = "DefineParameter")]
    private void defineParameter(string key, string title, [LuaCustomType(typeof(ParameterDefinitionType))] string type, object? fallback = default) =>
        DefineParameter?.Invoke(key, title, type, fallback);
#nullable disable

    [LuaGlobal]
    private void print(string text) => Logger.Add($"[Script] {text}");
}
