using Newtonsoft.Json;

namespace fluXis.Game.Online.Fluxel.Packets.Chat;

public class ChatHistoryPacket : Packet
{
    [JsonProperty("channel")]
    public string Channel { get; set; }

    public ChatHistoryPacket()
        : base(11)
    {
    }
}
