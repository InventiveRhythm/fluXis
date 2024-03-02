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
}
