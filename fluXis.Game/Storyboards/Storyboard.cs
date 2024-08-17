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
}
