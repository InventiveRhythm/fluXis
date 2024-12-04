using System;
using fluXis.Game.Utils;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Payloads.Auth;

#nullable enable

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
