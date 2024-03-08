using Newtonsoft.Json;

namespace fluXis.Shared.API.Packets.Chat;

public class ChatDeletePacket : IPacket
{
    public string ID => PacketIDs.CHAT_MESSAGE;

    [JsonProperty("id")]
    public string? MessageID { get; set; }

    public static ChatDeletePacket Create(string id) => new() { MessageID = id };
}
