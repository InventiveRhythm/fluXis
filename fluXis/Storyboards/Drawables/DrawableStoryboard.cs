using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fluXis.Configuration;
using fluXis.Map;
using fluXis.Screens;
using fluXis.Screens.Edit;
using fluXis.Scripting;
using fluXis.Scripting.Models;
using fluXis.Scripting.Runners;
using fluXis.Skinning;
using fluXis.Storyboards.Storage;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace fluXis.Storyboards.Drawables;

public partial class DrawableStoryboard : CompositeDrawable
{
    [Resolved]
    private FluXisConfig config { get; set; }

    [Resolved]
    private ISkin skin { get; set; }

    [Resolved]
    private FluXisScreenStack screens { get; set; }

    [Resolved(CanBeNull = true)]
    private EditorMap editorMap { get; set; }

    public Storyboard Storyboard { get; }
    private MapInfo map { get; }
    private string assetPath { get; }

    public StoryboardStorage Storage { get; private set; }
    private Dictionary<string, StoryboardScriptRunner> scripts { get; } = new();

    public DrawableStoryboard(MapInfo map, Storyboard storyboard, string assetPath)
    {
        this.map = map;
        Storyboard = storyboard;
        this.assetPath = assetPath;
    }

    [BackgroundDependencyLoader]
    private void load(GameHost host)
    {
        Storage = new StoryboardStorage(host, assetPath);

        var elements = Storyboard.Elements.Where(e => e.Type == StoryboardElementType.Script).ToList();

        foreach (var element in elements)
        {
            var script = loadScript(element.GetParameter("path", ""));
            script?.Process(element);
        }

        Storyboard.Sort();
    }

    private StoryboardScriptRunner loadScript(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return null;

        if (scripts.TryGetValue(path, out var script))
            return script;

        var full = Storage.Storage.GetFullPath(path);

        if (!File.Exists(full))
            return null;
        
        var mapInfo = screens.CurrentScreen is Editor ? (editorMap?.MapInfo ?? map) : map;

        var raw = File.ReadAllText(full);
        var runner = scripts[path] = new StoryboardScriptRunner(mapInfo, Storyboard, new LuaSettings(config), skin);

        try
        {
            runner.Run(raw);
        }
        catch (Exception ex)
        {
            ScriptRunner.Logger.Add($"Failed to load script '{path}'.", LogLevel.Error, ex);
        }

        return runner;
    }
}
