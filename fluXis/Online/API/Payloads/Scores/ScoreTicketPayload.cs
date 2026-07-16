using System.Collections.Generic;
using Newtonsoft.Json;

namespace fluXis.Online.API.Payloads.Scores;

public class ScoreTicketPayload
{
    [JsonProperty("hash")]
    public string MapHash { get; set; }

    [JsonProperty("effect-hash")]
    public string EffectHash { get; set; }

    [JsonProperty("sb-hash")]
    public string StoryboardHash { get; set; }

    [JsonProperty("mods")]
    public List<string> Mods { get; set; }
}
