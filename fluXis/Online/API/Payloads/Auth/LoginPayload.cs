using System;
using System.ComponentModel.DataAnnotations;
using Midori.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace fluXis.Online.API.Payloads.Auth;

public class LoginPayload
{
    [JsonProperty("method")]
    [Required]
    public string Method { get; set; } = null!;

    [JsonProperty("data")]
    [Required]
    public JObject Data { get; set; } = null!;

    public LoginPayload(string method, JObject data)
    {
        Method = method;
        Data = data;
    }

    [JsonConstructor]
    [Obsolete(JsonUtils.JSON_CONSTRUCTOR_ERROR, true)]
    public LoginPayload()
    {
    }

    public static LoginPayload CreateSteam(string ticket) => new("steam", JObject.FromObject(new { ticket }));
    public static LoginPayload CreateLegacy(string username, string password) => new("legacy", JObject.FromObject(new { username, password }));
}
