#nullable enable

using System;
using Newtonsoft.Json;

namespace fluXis.Online.Fluxel;

public class FluxelReply<T>
{
    [JsonProperty("id")]
    public string ID { get; init; } = string.Empty;

    [JsonProperty("success")]
    public bool Success { get; set; }

    [JsonProperty("token")]
    public Guid Token { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; } = "OK";

    [JsonProperty("data")]
    public T? Data { get; set; }
}
