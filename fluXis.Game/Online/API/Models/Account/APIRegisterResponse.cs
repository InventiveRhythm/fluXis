using fluXis.Game.Online.API.Models.Users;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Account;

public class APIRegisterResponse
{
    [JsonProperty("token")]
    public string Token;

    [JsonProperty("user")]
    public APIUser User;
}
