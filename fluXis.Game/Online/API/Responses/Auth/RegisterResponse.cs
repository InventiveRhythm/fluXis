using System;
using fluXis.Game.Utils;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Responses.Auth;

public class RegisterResponse
{
    [JsonProperty("token")]
    public string AccessToken { get; init; } = null!;

    public RegisterResponse(string token)
    {
        AccessToken = token;
    }

    [JsonConstructor]
    [Obsolete(JsonUtils.JSON_CONSTRUCTOR_ERROR, true)]
    public RegisterResponse()
    {
    }
}
