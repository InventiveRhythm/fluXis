using fluXis.Shared.Utils;
using Newtonsoft.Json;

namespace fluXis.Shared.API.Responses.Auth;

public class LoginResponse
{
    [JsonProperty("token")]
    public string AccessToken { get; init; } = null!;

    [JsonProperty("uid")]
    public long UserID { get; init; }

    public LoginResponse(string token, long userID)
    {
        AccessToken = token;
        UserID = userID;
    }

    [JsonConstructor]
    [Obsolete(JsonUtils.JSON_CONSTRUCTOR_ERROR, true)]
    public LoginResponse()
    {
    }
}
