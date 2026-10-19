using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Map;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit;
using fluXis.Scripting;
using fluXis.Storyboards.Drawables;
using Midori.Utils;
using Newtonsoft.Json;
using osuTK;

namespace fluXis.Storyboards;

public class Storyboard : EditorMap.IChangeNotifier
{
    /// <summary>
    /// Version of the storyboard api.
    /// Has to start at V1 because old storyboards didn't have this field so, this is for updating old maps.
    /// </summary>
    [JsonProperty("version")]
    public Version Version { get; set; } = StoryboardApiVersion.V1;

    [JsonProperty("resolution")]
    public Vector2 Resolution { get; set; } = new(1920, 1080);

    [JsonProperty("elements")]
    public List<StoryboardElement> Elements { get; set; } = [];

    [JsonProperty("compounds")]
    public Dictionary<string, List<StoryboardElement>> Compounds { get; set; } = [];

    [JsonIgnore]
    public bool Empty => Elements.Count == 0 && Compounds.Count == 0;

    [JsonIgnore]
    private static readonly Dictionary<Version, Action<Storyboard>> migrations = new()
    {
        [StoryboardApiVersion.V2] = sb =>
        {
            sb.Elements.ForEach(e => e.Animations.ForEach(a => a.StartTime -= e.StartTime));
        },
    };

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

    public DrawableStoryboard CreateDrawable(PlayableMap map, osu.Framework.Platform.Storage storage)
        => new(map, this, storage.GetFullPath(string.Empty));

    /// <summary>
    /// Updates and migrates the storyboard to the latest known version.
    /// Check <see cref="Storyboard.migrations"/> for more info about the migrations.
    /// </summary>
    public void Update()
    {
        while (Version < StoryboardApiVersion.LATEST)
        {
            // Keep migrating to the next known version until the latest
            Version = StoryboardApiVersion.VERSIONS
                                          .Where(v => v > Version)
                                          .OrderBy(v => v)
                                          .First();

            if (migrations.TryGetValue(Version, out var migrate))
                migrate(this);
        }
    }

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

    public void ApplyOffset(double offset) => Elements.ForEach(x =>
    {
        x.StartTime += offset;
        x.EndTime += offset;
    });

    public List<StoryboardElement> GetScriptElements(string s) =>
        Elements.FindAll(e => e.Type == StoryboardElementType.Script && e.GetParameter("path", "") == s);

    public bool Matches(Type type) => typeof(StoryboardElement) == type;

    public string Save() => Sort().Serialize(true);
}
