using fluXis.Game.Online.Fluxel;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Packets.Chat;

public class ChatChannelLeavePacket : IPacket
{
    public string ID => PacketIDs.CHAT_LEAVE;

    [JsonProperty("channel")]
    public string Channel { get; init; } = null!;
}
