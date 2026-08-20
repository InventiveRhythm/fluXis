using System;
using System.ComponentModel.DataAnnotations;
using fluXis.Utils;
using Midori.Utils;
using Newtonsoft.Json;

namespace fluXis.Online.API.Payloads.Auth;

#nullable enable

public class RegisterPayload
{
    [JsonProperty("username")]
    [Required, RegularExpression(Validate.USERNAME)]
    public string Username { get; set; } = null!;

    [JsonProperty("steam")]
    [Required]
    public string SteamTicket { get; set; } = null!;

    public RegisterPayload(string username, string ticket)
    {
        Username = username;
        SteamTicket = ticket;
    }

    [JsonConstructor]
    [Obsolete(JsonUtils.JSON_CONSTRUCTOR_ERROR, true)]
    public RegisterPayload()
    {
    }
}
