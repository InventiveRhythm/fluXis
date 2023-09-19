using Newtonsoft.Json;

namespace fluXis.Game.Online.API;

public class APIResponse<T>
{
    [JsonProperty("code")]
    public int Status { get; init; }

    [JsonProperty("message")]
    public string Message { get; init; }

    [JsonProperty("data")]
    public T Data { get; init; }

    public APIResponse(int status, string message, T data)
    {
        Status = status;
        Message = message;
        Data = data;
    }
}
