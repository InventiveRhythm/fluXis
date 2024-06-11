using fluXis.Shared.Utils;
using Newtonsoft.Json;

namespace fluXis.Shared.API.Parameters.Auth;

public class LoginParameters
{
    [JsonProperty("username")]
    public string Username { get; set; } = null!;

    [JsonProperty("password")]
    public string Password { get; set; } = null!;

    public LoginParameters(string username, string password)
    {
        Username = username;
        Password = password;
    }

    [JsonConstructor]
    [Obsolete(JsonUtils.JSON_CONSTRUCTOR_ERROR, true)]
    public LoginParameters()
    {
    }
}
