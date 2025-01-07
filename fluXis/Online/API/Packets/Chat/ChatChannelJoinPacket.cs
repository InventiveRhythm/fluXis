using fluXis.Online.Fluxel;
using Newtonsoft.Json;

namespace fluXis.Online.API.Packets.Chat;

public class ChatChannelJoinPacket : IPacket
{
    public string ID => PacketIDs.CHAT_JOIN;

    [JsonProperty("channel")]
    public string Channel { get; init; } = null!;
}
