using Newtonsoft.Json;

namespace fluXis.Shared.Components.Maps;

public class APIMapVotes
{
    [JsonProperty("id")]
    public long MapID { get; set; }

    [JsonProperty("vote")]
    public int YourVote { get; set; }

    [JsonProperty("ups")]
    public long UpVotes { get; set; }

    [JsonProperty("downs")]
    public long DownVotes { get; set; }
}
