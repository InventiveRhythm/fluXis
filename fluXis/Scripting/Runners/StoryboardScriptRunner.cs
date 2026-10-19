using System;
using System.Collections.Generic;
using fluXis.Audio.FFT;
using fluXis.Graphics;
using fluXis.Map;
using fluXis.Scripting.Attributes;
using fluXis.Scripting.Models;
using fluXis.Scripting.Models.Skinning;
using fluXis.Scripting.Models.Storyboarding;
using fluXis.Scripting.Models.Storyboarding.Elements;
using fluXis.Skinning;
using fluXis.Storyboards;
using JetBrains.Annotations;
using NLua.Exceptions;
using osu.Framework.Graphics;
using osu.Framework.Logging;

namespace fluXis.Scripting.Runners;

[LuaDefinition("storyboard", Hide = true)]
public class StoryboardScriptRunner : ScriptRunner, IHasLoadedValue
{
    private readonly List<StoryboardElement> addTo;

    private readonly Storyboard storyboard;

    // This is used to keep compatibility between lua and the standard storyboard, like migrations for example.
    private Storyboard temporaryStoryboard;

    public bool Loaded { get; private set; }

    [LuaGlobal(Name = "screen")]
    public LuaVector2 ScreenResolution { get; }

    public StoryboardScriptRunner(
        PlayableMap map, [CanBeNull] AudioAnalyzer audioAnalyzer,
        Storyboard storyboard,
        LuaSettings settings,
        ISkin skin,
        List<StoryboardElement> addTo = null)
    {
        this.storyboard = storyboard;
        Map = map;

        this.addTo = addTo ?? this.storyboard.Elements;

        ScreenResolution = new LuaVector2(storyboard.Resolution);
        AddField("screen", ScreenResolution);
        AddField("metadata", new LuaMetadata(map));
        AddField("settings", settings);
        AddField("skin", new LuaSkin(skin));
        AddField("map", new LuaMap(map, Lua));
        AddField("AudioAnalyzer", new LuaAudioAnalyzer(audioAnalyzer, Lua));

        AddFunction("Add", add);

        // enums
        AddFunction("Layer", (string input) => Enum.TryParse(input, out StoryboardLayer layer) ? layer : StoryboardLayer.Background);
        AddFunction("Anchor", (string str) => Enum.TryParse(str, out Anchor anchor) ? anchor : Anchor.TopLeft);
        AddFunction("BlendMode", (string str) => Enum.TryParse(str, out DefaultBlendingParameters blendMode) ? blendMode : DefaultBlendingParameters.Mix);
        // AddFunction("HitObjectType", (string str) => Enum.TryParse(str, out HitObjectType hitType) ? hitType : HitObjectType.Normal);

        // elements
        AddFunction("StoryboardBox", newBox);
        AddFunction("StoryboardOutlineBox", newOutlineBox);
        AddFunction("StoryboardSprite", newSprite);
        AddFunction("StoryboardText", newText);
        AddFunction("StoryboardCircle", newCircle);
        AddFunction("StoryboardOutlineCircle", newOutlineCircle);
        AddFunction("StoryboardSkinSprite", newSkinSprite);
    }

    public void Process(StoryboardElement element)
    {
        try
        {
            temporaryStoryboard = new Storyboard { Version = Version };
            var func = GetFunction("process");

            if (func is null)
            {
                Logger.Add("Missing process(parent) function!", LogLevel.Error);
                return;
            }

            var l = LuaStoryboardElement.FromElement(element);
            func.Call(l);

            // update local storyboard and add to the target storyboard - completing any migrations
            temporaryStoryboard.Update();
            addTo.AddRange(temporaryStoryboard.Elements);

            Loaded = true;
        }
        catch (Exception ex)
        {
            Logger.Add("Error when running process()!", LogLevel.Error, ex);

            if (ex is LuaException lx)
                Logger.Add(lx.Message, LogLevel.Error);
        }

        temporaryStoryboard = null;
    }

    [LuaGlobal(Name = "Add")]
    private void add(LuaStoryboardElement element)
    {
        temporaryStoryboard.Elements.Add(element.Build());
    }

    [LuaGlobal(Name = "StoryboardBox")]
    private LuaStoryboardBox newBox() => new();

    [LuaGlobal(Name = "StoryboardOutlineBox")]
    private LuaStoryboardOutlineBox newOutlineBox() => new();

    [LuaGlobal(Name = "StoryboardSprite")]
    private LuaStoryboardSprite newSprite() => new();

    [LuaGlobal(Name = "StoryboardText")]
    private LuaStoryboardText newText() => new();

    [LuaGlobal(Name = "StoryboardCircle")]
    private LuaStoryboardCircle newCircle() => new();

    [LuaGlobal(Name = "StoryboardOutlineCircle")]
    private LuaStoryboardOutlineCircle newOutlineCircle() => new();

    [LuaGlobal(Name = "StoryboardSkinSprite")]
    private LuaStoryboardSkinSprite newSkinSprite([LuaCustomType(typeof(SkinSprite))] string str)
        => new(Enum.TryParse<SkinSprite>(str, out var s) ? s : SkinSprite.HitObject);
}
