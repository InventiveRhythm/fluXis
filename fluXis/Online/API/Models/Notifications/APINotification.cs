using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace fluXis.Online.API.Models.Notifications;

public class APINotification
{
    [JsonProperty("type")]
    public NotificationType Type { get; set; }

    [JsonProperty("data")]
    public JObject Data { get; set; }

    [JsonProperty("time")]
    public long Time { get; set; }

    public T GetDataAs<T>() where T : class
        => Data.ToObject<T>();
}
