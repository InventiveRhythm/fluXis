using System.Collections.Generic;
using Newtonsoft.Json;

namespace fluXis.Game.Storyboards;

public class Storyboard
{
    [JsonProperty("elements")]
    public List<StoryboardElement> Elements { get; set; } = new();
}
