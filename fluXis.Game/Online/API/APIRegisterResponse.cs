using Newtonsoft.Json;

namespace fluXis.Game.Online.API;

public class APIRegisterResponse
{
    [JsonProperty("token")]
    public string Token;

    [JsonProperty("user")]
    public APIUser User;
}
