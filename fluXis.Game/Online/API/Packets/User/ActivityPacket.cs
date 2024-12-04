using fluXis.Game.Online.Fluxel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace fluXis.Game.Online.API.Packets.User;

public class ActivityPacket : IPacket
{
    public string ID => PacketIDs.ACTIVITY;

    [JsonProperty("activity")]
    public string Activity { get; set; } = null!;

    [JsonProperty("data")]
    public JObject Data { get; set; } = null!;

    public static ActivityPacket CreateC2S(string activity, JObject data) => new()
    {
        Activity = activity,
        Data = data
    };
}
