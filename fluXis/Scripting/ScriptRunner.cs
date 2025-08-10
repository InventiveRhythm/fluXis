using System;
using System.Text;
using fluXis.Map;
using fluXis.Scripting.Attributes;
using fluXis.Scripting.Models;
using JetBrains.Annotations;
using NLua;
using osu.Framework.Logging;
using osu.Framework.Utils;

namespace fluXis.Scripting;

[LuaDefinition("shared", Hide = true)]
public class ScriptRunner
{
    public static Logger Logger { get; } = Logger.GetLogger("scripting");

    [CanBeNull]
    protected MapInfo Map { get; set; }

    public Action<string, string, string> DefineParameter;
    protected Lua Lua { get; }

    protected ScriptRunner()
    {
        Lua = new Lua();
        Lua.State.Encoding = Encoding.UTF8;

        AddField("mathf", new LuaMath());

        AddFunction("print", print);
        AddFunction("RandomRange", randomRange);
        AddFunction("Vector2", (float x, float y) => new LuaVector2(x, y));
        AddFunction("BPMAtTime", findBpm);

        AddFunction("DefineParameter", defineParameter);

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

    [LuaGlobal(Name = "RandomRange")]
    private int randomRange(int from, int to) => RNG.Next(from, to + 1);

    [LuaGlobal(Name = "BPMAtTime")]
    private float findBpm(double time)
    {
        var point = Map?.GetTimingPoint(time) ?? throw new InvalidOperationException("Tried to call findBpm without a map!");
        return point.BPM;
    }

    [LuaGlobal(Name = "DefineParameter")]
    private void defineParameter(string key, string title, [LuaCustomType(typeof(ParameterDefinitionType))] string type) => DefineParameter?.Invoke(key, title, type);

    [LuaGlobal]
    private void print(string text) => Logger.Add($"[Script] {text}");
}
