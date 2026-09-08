using fluXis.Online.API.Models.Maps;
using fluXis.Online.API.Models.Maps.Modding;
using Newtonsoft.Json;

namespace fluXis.Online.API.Models.Notifications.Data;

public class QueueUpdateNotification
{
    [JsonProperty("set")]
    public APIMapSet MapSet { get; set; }

    [JsonProperty("type")]
    public APIModdingActionType Type { get; set; }
}
