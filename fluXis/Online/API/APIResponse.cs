using Newtonsoft.Json;

namespace fluXis.Online.API;

public class APIResponse<T>
{
    [JsonProperty("status")]
    public int Status { get; init; }

    [JsonProperty("message")]
    public string Message { get; init; }

    [JsonProperty("data")]
    public T Data { get; init; }

    [JsonIgnore]
    public bool Success => Status is >= 200 and < 300;

    public APIResponse(int status, string message, T data)
    {
        Status = status;
        Message = message;
        Data = data;
    }
}
