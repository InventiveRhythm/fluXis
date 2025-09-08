using Newtonsoft.Json;

namespace fluXis.Online.API.Models.Maps;

public class APIMapSetLoveState
{
    [JsonProperty("loved")]
    public bool Loved { get; set; }
}
