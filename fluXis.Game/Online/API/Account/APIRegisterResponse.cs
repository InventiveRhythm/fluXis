using fluXis.Game.Online.API.Users;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Account;

public class APIRegisterResponse
{
    [JsonProperty("token")]
    public string Token;

    [JsonProperty("user")]
    public APIUser User;
}
