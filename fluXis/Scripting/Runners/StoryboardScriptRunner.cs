using System;
using fluXis.Map;
using fluXis.Scripting.Attributes;
using fluXis.Scripting.Models;
using fluXis.Scripting.Models.Storyboarding;
using fluXis.Scripting.Models.Storyboarding.Elements;
using fluXis.Storyboards;
using osu.Framework.Graphics;
using osu.Framework.Logging;

namespace fluXis.Scripting.Runners;

[LuaDefinition("storyboard", Hide = true)]
public class StoryboardScriptRunner : ScriptRunner
{
    private readonly Storyboard storyboard;

    [LuaGlobal(Name = "screen")]
    public LuaVector2 ScreenResolution { get; }

    public StoryboardScriptRunner(MapInfo map, Storyboard storyboard)
    {
        this.storyboard = storyboard;
        Map = map;

        ScreenResolution = new LuaVector2(storyboard.Resolution);
        AddField("screen", ScreenResolution);
        AddField("metadata", new LuaMetadata(map));

        AddFunction("Add", add);

        // enums
        AddFunction("Layer", (string input) => Enum.TryParse(input, out StoryboardLayer layer) ? layer : StoryboardLayer.Background);
        AddFunction("Anchor", (string str) => Enum.TryParse(str, out Anchor anchor) ? anchor : Anchor.TopLeft);

        // elements
        AddFunction("StoryboardBox", newBox);
        AddFunction("StoryboardSprite", newSprite);
        AddFunction("StoryboardText", newText);
        AddFunction("StoryboardCircle", newCircle);
        AddFunction("StoryboardOutlineCircle", newOutlineCircle);
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

    [LuaGlobal(Name = "Add")]
    private void add(LuaStoryboardElement element) => storyboard.Elements.Add(element.Build());

    [LuaGlobal(Name = "StoryboardBox")]
    private LuaStoryboardBox newBox() => new();

    [LuaGlobal(Name = "StoryboardSprite")]
    private LuaStoryboardSprite newSprite() => new();

    [LuaGlobal(Name = "StoryboardText")]
    private LuaStoryboardText newText() => new();

    [LuaGlobal(Name = "StoryboardCircle")]
    private LuaStoryboardCircle newCircle() => new();

    [LuaGlobal(Name = "StoryboardOutlineCircle")]
    private LuaStoryboardOutlineCircle newOutlineCircle() => new();
}
