using fluXis.Game.Scripting.Models.Storyboarding;
using fluXis.Game.Scripting.Models.Storyboarding.Elements;
using fluXis.Game.Storyboards;
using osu.Framework.Logging;

namespace fluXis.Game.Scripting.Runners;

public class StoryboardScriptRunner : ScriptRunner
{
    private readonly Storyboard storyboard;

    public StoryboardScriptRunner(Storyboard storyboard)
    {
        this.storyboard = storyboard;

        AddFunction("add", add);
        AddFunction("StoryboardBox", () => new LuaStoryboardBox());
        AddFunction("StoryboardSprite", () => new LuaStoryboardSprite());
        AddFunction("StoryboardText", () => new LuaStoryboardText());
    }

    public void Process(StoryboardElement element)
    {
        var func = Lua.GetFunction("process");

        if (func is null)
        {
            Logger.Add("Missing process(parent) function!", LogLevel.Error);
            return;
        }

        var l = LuaStoryboardElement.FromElement(element);
        func.Call(l);
    }

    private void add(LuaStoryboardElement element)
        => storyboard.Elements.Add(element.Build());
}
