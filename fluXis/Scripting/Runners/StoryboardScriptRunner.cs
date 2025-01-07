using System;
using fluXis.Scripting.Models.Storyboarding;
using fluXis.Scripting.Models.Storyboarding.Elements;
using fluXis.Storyboards;
using osu.Framework.Graphics;
using osu.Framework.Logging;

namespace fluXis.Scripting.Runners;

public class StoryboardScriptRunner : ScriptRunner
{
    private readonly Storyboard storyboard;

    public StoryboardScriptRunner(Storyboard storyboard)
    {
        this.storyboard = storyboard;

        AddFunction("Add", add);

        // enums
        AddFunction("Layer", (string str) => Enum.TryParse(str, out StoryboardLayer layer) ? layer : StoryboardLayer.Background);
        AddFunction("Anchor", (string str) => Enum.TryParse(str, out Anchor anchor) ? anchor : Anchor.TopLeft);

        // elements
        AddFunction("StoryboardBox", () => new LuaStoryboardBox());
        AddFunction("StoryboardSprite", () => new LuaStoryboardSprite());
        AddFunction("StoryboardText", () => new LuaStoryboardText());
    }

    public void Process(StoryboardElement element)
    {
        try
        {
            var func = GetFunction("process");

            if (func is null)
            {
                Logger.Add("Missing process(parent) function!", LogLevel.Error);
                return;
            }

            var l = LuaStoryboardElement.FromElement(element);
            func.Call(l);
        }
        catch (Exception ex)
        {
            Logger.Add("Error when running process()!", LogLevel.Error, ex);
        }
    }

    private void add(LuaStoryboardElement element) => storyboard.Elements.Add(element.Build());
}
