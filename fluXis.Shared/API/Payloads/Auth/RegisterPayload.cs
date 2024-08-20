using fluXis.Shared.Utils;
using Newtonsoft.Json;

namespace fluXis.Shared.API.Payloads.Auth;

public class RegisterPayload
{
    [JsonProperty("username")]
    public string Username { get; set; } = null!;

    [JsonProperty("password")]
    public string Password { get; set; } = null!;

    [JsonProperty("email")]
    public string Email { get; set; } = null!;

    public RegisterPayload(string username, string password, string email)
    {
        Username = username;
        Password = password;
        Email = email;
    }

    [JsonConstructor]
    [Obsolete(JsonUtils.JSON_CONSTRUCTOR_ERROR, true)]
    public RegisterPayload()
    {
    }
}
