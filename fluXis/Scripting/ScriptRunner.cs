using System;
using System.Text;
using fluXis.Map;
using fluXis.Scripting.Attributes;
using fluXis.Scripting.Models;
using JetBrains.Annotations;
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
    protected VersionedLua VersionedLua { get; }
    protected int Version { get; private set; } = 1;

    protected ScriptRunner()
    {
        Lua = new Lua();
        Lua.State.Encoding = Encoding.UTF8;
        VersionedLua = new VersionedLua(Lua) { Version = Version };
    }

    protected virtual void Initialize()
    {
        VersionedLua.AddField("mathf", new LuaMath());
        VersionedLua.AddField("FFTParameters", new LuaFFTParameters());

        VersionedLua.AddFunction("print", print);
        VersionedLua.AddFunction("RandomRange", randomRange);
        VersionedLua.AddFunction("Easing", (string str) => Enum.TryParse(str, out Easing easing) ? easing : Easing.None);
        VersionedLua.AddFunction("Vector2", (float x, float y) => new LuaVector2(x, y));
        VersionedLua.AddFunction("Color4", (float r, float g, float b, float a) => new LuaColor4(r, g, b, a));
        VersionedLua.AddFunction("Vector2Zero", () => new LuaVector2(0, 0));
        VersionedLua.AddFunction("Vector2One", () => new LuaVector2(1, 1));
        VersionedLua.AddFunction("BPMAtTime", findBpm);

        VersionedLua.AddFunction("DefineParameter", defineParameter);
        VersionedLua.AddFunction("SetVersion", setVersion);

        Lua.DoString("import = function() end"); // disable importing
    }

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

    public void Run(string code)
    {
        Initialize();
        VersionedLua.Resolve();
        Lua.DoString(code);
    }

    /// <summary>
    /// Sets the version of the script allowing to use an older api or a newer one.
    /// MUST be at the top of the file after env.
    /// </summary>
    /// <param name="v">The version to use</param>
    /// <exception cref="ArgumentException"></exception>
    [LuaGlobal(Name = "SetVersion")]
    private void setVersion(double v)
    {
        // TODO:
        // This currently uses StoryboardApiVersion for version validation.
        // Make sure to change the validation when we eventually have scripting for stuff other than storyboard.

        int version = (int)v;

        if (!StoryboardApiVersion.IsValid(version, out var err))
            throw new ArgumentException(err);

        VersionedLua.Version = Version = version;
        VersionedLua.Resolve();
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
