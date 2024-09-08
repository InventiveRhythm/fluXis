using Newtonsoft.Json;

namespace fluXis.Shared.API.Payloads.Maps;

public class MapVotePayload
{
    [JsonProperty("vote")]
    public int YourVote { get; set; }
}
