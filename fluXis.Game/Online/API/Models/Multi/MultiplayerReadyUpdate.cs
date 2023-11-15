using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Multi;

public class MultiplayerReadyUpdate
{
    [JsonProperty("player_id")]
    public int PlayerID { get; init; }

    [JsonProperty("ready")]
    public bool Ready { get; init; }

    [JsonIgnore]
    public bool AllReady => PlayerID == -1;
}
