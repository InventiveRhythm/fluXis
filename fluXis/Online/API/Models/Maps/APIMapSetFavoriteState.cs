using Newtonsoft.Json;

namespace fluXis.Online.API.Models.Maps;

public class APIMapSetFavoriteState
{
    [JsonProperty("favorite")]
    public bool Favorite { get; set; }
}
