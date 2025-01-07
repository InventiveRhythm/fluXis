using System;
using fluXis.Utils;
using Newtonsoft.Json;

namespace fluXis.Online.API.Responses.Auth.Multifactor;

public class TOTPVerifyResponse
{
    [JsonProperty("token")]
    public string Token { get; init; } = null!;

    public TOTPVerifyResponse(string token)
    {
        Token = token;
    }

    [JsonConstructor]
    [Obsolete(JsonUtils.JSON_CONSTRUCTOR_ERROR, true)]
    public TOTPVerifyResponse()
    {
    }
}
