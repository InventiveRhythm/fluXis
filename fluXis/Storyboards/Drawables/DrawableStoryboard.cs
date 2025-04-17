using System.Collections.Generic;
using System.IO;
using System.Linq;
using fluXis.Map;
using fluXis.Scripting.Runners;
using fluXis.Storyboards.Storage;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Framework.Platform;

namespace fluXis.Storyboards.Drawables;

public partial class DrawableStoryboard : CompositeDrawable
{
    public Storyboard Storyboard { get; }
    private MapInfo map { get; }
    private string assetPath { get; }

    private StoryboardStorage storage { get; set; }
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
        storage = new StoryboardStorage(host, assetPath);

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

        var full = storage.Storage.GetFullPath(path);

        if (!File.Exists(full))
            return null;

        var raw = File.ReadAllText(full);
        var runner = scripts[path] = new StoryboardScriptRunner(map, Storyboard);
        runner.Run(raw);
        return runner;
    }

    public DrawableStoryboardLayer GetLayer(StoryboardLayer layer, int z)
        => new(storage, Storyboard.Elements.Where(e => e.Layer == layer && e.ZIndex == z).ToList());
}
