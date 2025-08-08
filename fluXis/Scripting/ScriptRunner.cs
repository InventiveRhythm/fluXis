using System;
using fluXis.Scripting.Models;
using JetBrains.Annotations;
using NLua;
using osu.Framework.Logging;
using osu.Framework.Utils;

namespace fluXis.Scripting;

public class ScriptRunner
{
    public static Logger Logger { get; } = Logger.GetLogger("scripting");

    public Action<string, string, string> DefineParameter;
    protected Lua Lua { get; }

    protected ScriptRunner()
    {
        Lua = new Lua();

        AddField("mathf", new LuaMath());

        AddFunction("print", (string text) => Logger.Add($"[Script] {text}"));
        AddFunction("RandomRange", (int from, int to) => RNG.Next(from, to + 1));
        
        AddFunction("Vector2", (float x, float y) => new LuaVector(x, y));
        AddFunction("Vector2Zero", () => new LuaVector(0, 0));
        AddFunction("Vector2One", () => new LuaVector(1, 1));

        AddFunction("DefineParameter", (string k, string t, string ty) => DefineParameter?.Invoke(k, t, ty));

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
}
