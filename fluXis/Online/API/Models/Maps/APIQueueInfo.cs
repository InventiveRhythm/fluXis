using System.Collections.Generic;
using fluXis.Online.API.Models.Maps.Modding;
using Newtonsoft.Json;

namespace fluXis.Online.API.Models.Maps;

#nullable enable

public class APIQueueInfo
{
    [JsonProperty("votes")]
    public List<bool> Votes { get; set; } = new();

    [JsonProperty("last_action")]
    public APIModdingAction? LastAction { get; set; }
}
