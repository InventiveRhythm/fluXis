using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Payloads.Maps;

public class MapVotePayload
{
    [JsonProperty("vote")]
    public int YourVote { get; set; }
}
