using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using osuTK;

namespace fluXis.Game.Storyboards;

public class Storyboard
{
    [JsonProperty("resolution")]
    public Vector2 Resolution { get; set; } = new(1920, 1080);

    [JsonProperty("elements")]
    public List<StoryboardElement> Elements { get; set; } = new();

    [JsonIgnore]
    public bool Empty => Elements.Count == 0;

    #region Server-Side Stuff

    [JsonIgnore]
    public string RawContent { get; set; } = "";

    #endregion

    public event Action<StoryboardElement> ElementAdded;
    public event Action<StoryboardElement> ElementRemoved;

    public void Add(StoryboardElement element)
    {
        Elements.Add(element);
        ElementAdded?.Invoke(element);
    }

    public void Remove(StoryboardElement element)
    {
        Elements.Remove(element);
        ElementRemoved?.Invoke(element);
    }

    public void Sort()
    {
        Elements.Sort((a, b) => a.StartTime.CompareTo(b.StartTime));
    }
}
