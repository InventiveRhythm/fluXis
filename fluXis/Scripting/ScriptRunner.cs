using System;
using fluXis.Scripting.Models;
using JetBrains.Annotations;
using NLua;
using osu.Framework.Logging;
using osu.Framework.Utils;

namespace fluXis.Scripting;

public class ScriptRunner
{
    protected static Logger Logger { get; } = Logger.GetLogger("scripting");

    protected Lua Lua { get; }

    protected ScriptRunner()
    {
        Lua = new Lua();

        AddField("mathf", new LuaMath());

        AddFunction("print", (string text) => Logger.Add($"[Script] {text}"));
        AddFunction("RandomRange", (int from, int to) => RNG.Next(from, to + 1));
        AddFunction("Vector2", (float x, float y) => new LuaVector(x, y));

        Lua.DoString("import = function() end"); // disable importing
    }

    protected void AddFunction(string name, Delegate function)
    {
        Lua.RegisterFunction(name, function.Target, function.Method);
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

    protected void AddField(string name, ILuaModel value) => Lua[name] = value;

    public void Run(string code)
    {
        Lua.DoString(code);
    }
}
