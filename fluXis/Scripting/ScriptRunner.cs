using System;
using System.Globalization;
using System.Text;
using fluXis.Map;
using fluXis.Scripting.Attributes;
using fluXis.Scripting.Models;
using JetBrains.Annotations;
using Midori.Utils.Extensions;
using NLua;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using osu.Framework.Utils;

namespace fluXis.Scripting;

[LuaDefinition("shared", Hide = true)]
public class ScriptRunner
{
    public static Logger Logger { get; } = Logger.GetLogger("scripting");

    [CanBeNull]
    protected PlayableMap Map { get; set; }

    public Action<string, string, string, object> DefineParameter;
    protected Lua Lua { get; }
    protected Version Version { get; private set; }

    protected ScriptRunner()
    {
        Lua = new Lua();
        Lua.State.Encoding = Encoding.UTF8;

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

        // Lua.DoString("import = function() end"); // disable importing
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

    /// <summary>
    /// Sets the version of the script.
    /// It's recommened to use strings for the parameter
    /// ex. `SetVersion("2.1")`
    /// MUST be at the top of the file after env.
    /// </summary>
    /// <param name="v"></param>
    /// <exception cref="ArgumentException"></exception>
    [LuaGlobal(Name = "SetVersion")]
    private void setVersion(object v)
    {
        // TODO:
        // This currently uses StoryboardApiVersion for version validation.
        // Make sure to change the validation when we eventually have scripting for stuff other than storyboard.

        string text = v switch
        {
            string s => s,
            double d => d.ToStringInvariant(),
            float f => ((decimal)f).ToString(CultureInfo.InvariantCulture),
            _ => throw new ArgumentException($"Unsupported version type: {v?.GetType()}")
        };

        Version = StoryboardApiVersion.Parse(text);
    }

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
