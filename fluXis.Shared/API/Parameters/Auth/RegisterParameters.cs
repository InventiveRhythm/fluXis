using Newtonsoft.Json;

namespace fluXis.Shared.API.Parameters.Auth;

public class RegisterParameters
{
    [JsonProperty("username")]
    public string Username { get; set; } = null!;

    [JsonProperty("password")]
    public string Password { get; set; } = null!;

    [JsonProperty("email")]
    public string Email { get; set; } = null!;

    public RegisterParameters(string username, string password, string email)
    {
        Username = username;
        Password = password;
        Email = email;
    }

    [JsonConstructor]
    [Obsolete("This constructor is for json parsing only.")]
    public RegisterParameters()
    {
    }
}
