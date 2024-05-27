using Newtonsoft.Json;

namespace fluXis.Shared.API.Responses.Auth;

public class RegisterResponse
{
    [JsonProperty("token")]
    public string AccessToken { get; init; } = null!;

    public RegisterResponse(string token)
    {
        AccessToken = token;
    }

    [JsonConstructor]
    [Obsolete("This constructor is for json parsing only.")]
    public RegisterResponse()
    {
    }
}
