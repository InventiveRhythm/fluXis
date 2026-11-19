using Newtonsoft.Json;

namespace fluXis.Online.API.Models.Notifications;

public class APINotificationList
{
    [JsonProperty("notifications")]
    public APINotification[] Notifications { get; set; } = [];

    [JsonProperty("last")]
    public long LastRead { get; set; }
}
