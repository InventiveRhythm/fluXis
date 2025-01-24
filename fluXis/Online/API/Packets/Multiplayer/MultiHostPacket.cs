using fluXis.Online.Fluxel;
using Newtonsoft.Json;

namespace fluXis.Online.API.Packets.Multiplayer;

#nullable enable

public class MultiHostPacket : IPacket
{
    public string ID => PacketIDs.MULTIPLAYER_HOST;

    [JsonProperty("user")]
    public long? UserID { get; set; }
}
