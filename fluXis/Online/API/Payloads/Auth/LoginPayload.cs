using System;
using fluXis.Utils;
using Newtonsoft.Json;

namespace fluXis.Online.API.Payloads.Auth;

public class LoginPayload
{
    [JsonProperty("username")]
    public string Username { get; set; } = null!;

    [JsonProperty("password")]
    public string Password { get; set; } = null!;

    public LoginPayload(string username, string password)
    {
        Username = username;
        Password = password;
    }

    [JsonConstructor]
    [Obsolete(JsonUtils.JSON_CONSTRUCTOR_ERROR, true)]
    public LoginPayload()
    {
    }
}
