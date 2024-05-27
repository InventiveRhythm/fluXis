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
    [Obsolete("This constructor is for json parsing only.")]
    public LoginResponse()
    {
    }
}
