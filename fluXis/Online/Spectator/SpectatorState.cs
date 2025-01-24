using Newtonsoft.Json;

namespace fluXis.Online.Spectator;

public class SpectatorState
{
    [JsonProperty("map")]
    public long? MapID { get; set; }
}
