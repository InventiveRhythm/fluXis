using Newtonsoft.Json;

namespace fluXis.Shared.API.Packets.Chat;

public class ChatChannelLeavePacket : IPacket
{
    public string ID => PacketIDs.CHAT_LEAVE;

    [JsonProperty("channel")]
    public string Channel { get; init; } = null!;
}
