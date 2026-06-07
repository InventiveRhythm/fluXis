using System.Collections.Generic;
using Newtonsoft.Json;

namespace fluXis.Online.Spectator.Models;

public class SpectatorState
{
    [JsonProperty("map")]
    public long? MapID { get; set; }

    [JsonProperty("mods")]
    public List<string> Mods { get; set; } = new();
}
