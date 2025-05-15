using System.Collections.Generic;
using Newtonsoft.Json;

namespace fluXis.Online.API.Models.Notifications;

public class APINotificationList
{
    [JsonProperty("notifications")]
    public List<APINotification> Notifications { get; set; }

    [JsonProperty("last")]
    public long LastRead { get; set; }
}
