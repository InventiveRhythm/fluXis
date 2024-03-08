using fluXis.Shared.Components.Chat;
using Newtonsoft.Json;

namespace fluXis.Shared.API.Packets.Chat;

public class ChatHistoryPacket : IPacket
{
    public string ID => PacketIDs.CHAT_HISTORY;

    [JsonProperty("channel")]
    public string? Channel { get; set; }

    #region Server2Client

    [JsonProperty("messages")]
    public List<IChatMessage>? Messages { get; set; }

    #endregion

    public static ChatHistoryPacket CreateC2S(string channel)
        => new() { Channel = channel };

    public static ChatHistoryPacket CreateS2C(string channel, List<IChatMessage> messages)
        => new() { Channel = channel, Messages = messages };
}
