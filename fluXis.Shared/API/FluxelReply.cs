using Newtonsoft.Json;

namespace fluXis.Shared.API;

public class FluxelReply<T>
{
    [JsonProperty("id")]
    public string ID { get; init; } = string.Empty;

    [JsonProperty("success")]
    public bool Success { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; } = "OK";

    [JsonProperty("data")]
    public T? Data { get; set; }
}
