using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fluXis.Map;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit;
using fluXis.Scripting.Runners;
using fluXis.Utils;
using Newtonsoft.Json;
using osuTK;

namespace fluXis.Storyboards;

public class Storyboard : EditorMap.IChangeNotifier
{
    [JsonProperty("resolution")]
    public Vector2 Resolution { get; set; } = new(1920, 1080);

    [JsonProperty("elements")]
    public List<StoryboardElement> Elements { get; set; } = new();

    private HashSet<string> cachedUsedScriptAssets { get; } = new();

    [JsonIgnore]
    public bool Empty => Elements.Count == 0;

    #region Server-Side Stuff

    [JsonIgnore]
    public string RawContent { get; set; } = "";

    #endregion

    public event Action<StoryboardElement> ElementAdded;
    public event Action<StoryboardElement> ElementRemoved;
    public event Action<StoryboardElement> ElementUpdated;

    public event Action<ITimedObject> OnAdd;
    public event Action<ITimedObject> OnRemove;
    public event Action<ITimedObject> OnUpdate;

    public Storyboard Sort()
    {
        Elements.Sort((a, b) => a.StartTime.CompareTo(b.StartTime));
        return this;
    }

    public void Add(ITimedObject obj)
    {
        var element = (StoryboardElement)obj;

        Elements.Add(element);
        ElementAdded?.Invoke(element);
        OnAdd?.Invoke(element);
    }

    public void Remove(ITimedObject obj)
    {
        var element = (StoryboardElement)obj;

        Elements.Remove(element);
        ElementRemoved?.Invoke(element);
        OnRemove?.Invoke(element);
    }

    public void Update(ITimedObject obj)
    {
        var element = (StoryboardElement)obj;

        ElementUpdated?.Invoke(element);
        OnUpdate?.Invoke(obj);
    }

    public void ApplyOffset(float offset) => Elements.ForEach(x =>
    {
        x.StartTime += offset;
        x.EndTime += offset;
    });

    public HashSet<string> GetUsedScriptAssets(MapInfo map, string assetPath)
    {
        if (cachedUsedScriptAssets.Count > 0)
            return cachedUsedScriptAssets;

        var scriptElements = Elements.Where(e => e.Type == StoryboardElementType.Script).ToList();

        foreach (var scriptElement in scriptElements)
        {
            var directory = Path.GetDirectoryName(assetPath);
            var full = Path.Combine(directory, scriptElement.GetParameter("path", ""));
            var raw = File.ReadAllText(full);

            var script = new StoryboardScriptRunner(map, this);
            script.Run(raw);
            script?.Process(scriptElement);

            var luaSpriteElements = script.GetScriptStoryboardElements()
                                        .Where(e => e.Type == StoryboardElementType.Sprite)
                                        .ToList();

            foreach (var element in luaSpriteElements)
                cachedUsedScriptAssets.Add(element.GetParameter("file", ""));
        }

        return cachedUsedScriptAssets;
    }

    public bool Matches(Type type) => typeof(StoryboardElement) == type;

    public string Save() => Sort().Serialize();
}
