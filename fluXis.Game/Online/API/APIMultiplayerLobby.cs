using Newtonsoft.Json;

namespace fluXis.Game.Online.API;

public class APIMultiplayerLobby
{
    [JsonProperty("id")]
    public int ID { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("max")]
    public int MaxPlayers { get; set; }

    [JsonProperty("current")]
    public int CurrentPlayers { get; set; }

    [JsonProperty("password")]
    public bool PasswordProtected { get; set; }

    [JsonProperty("owner")]
    public APIUserShort OwnerID { get; set; }

    [JsonProperty("players")]
    public APIUserShort[] Players { get; set; }
}
