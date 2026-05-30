using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fluXis.Audio.FFT;
using fluXis.Configuration;
using fluXis.Map;
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
    protected FluXisConfig Config { get; private set; }

    [Resolved]
    protected ISkin Skin { get; private set; }

    [Resolved]
    protected AudioAnalyzer AudioAnalyzer { get; private set; }

    public Storyboard Storyboard { get; }
    protected MapInfo Map { get; }
    private string assetPath { get; }

    public StoryboardStorage Storage { get; private set; }
    private Dictionary<string, StoryboardScriptRunner> scripts { get; } = new();

    public DrawableStoryboard(MapInfo map, Storyboard storyboard, string assetPath)
    {
        Map = map;
        Storyboard = storyboard;
        this.assetPath = assetPath;
    }

    [BackgroundDependencyLoader]
    private void load(GameHost host)
    {
        Storage = new StoryboardStorage(host, assetPath);

        // wait for FFT data to be ready before scripts can get amplitudes.
        try
        {
            AudioAnalyzer.ComputeComplete.Wait();
        }
        catch (OperationCanceledException)
        {
            return;
        }

        LoadScripts();
        Storyboard.Sort();
    }

    protected virtual void LoadScripts()
    {
        var elements = Storyboard.Elements.Where(e => e.Type == StoryboardElementType.Script).ToList();

        foreach (var element in elements)
        {
            var script = LoadScript(element.GetParameter("path", ""));
            script?.Process(element);
        }
    }

    protected StoryboardScriptRunner LoadScript(string path, List<StoryboardElement> addTo = null)
    {
        if (string.IsNullOrWhiteSpace(path))
            return null;

        // Only use the cache when not adding into a dedicated list
        if (addTo == null && scripts.TryGetValue(path, out var cached))
            return cached;

        var full = Storage.Storage.GetFullPath(path);

        if (!File.Exists(full))
            return null;

        var raw = File.ReadAllText(full);
        var runner = new StoryboardScriptRunner(Map, AudioAnalyzer, Storyboard, new LuaSettings(Config), Skin, addTo);

        if (addTo == null)
            scripts[path] = runner;

        try
        {
            runner.Run(raw);
        }
        catch (Exception ex)
        {
            ScriptRunner.Logger.Add($"Failed to load script '{path}'.", LogLevel.Error, ex);
            return null;
        }

        return runner;
    }
}
